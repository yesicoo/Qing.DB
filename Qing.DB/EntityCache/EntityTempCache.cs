using Qing.DB.Interfaces;
using Qing.DB.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    /// <summary>
    /// 临时缓存，指定一个时间间隔，再一定范围内没有操作 就会释放掉，再次操作的时候会重新加载
    /// </summary>
    public class EntityTempCache<T> : IEntityCache<T> where T : BaseEntity
    {

        ConcurrentDictionary<TimeKey, List<T>> _dts = new ConcurrentDictionary<TimeKey, List<T>>();
        QingEntityAttribute nea = Tools.GetNEA(typeof(T));
        Expression<Func<T, bool>> _loadExpression = null;
        Expression<Func<T, object>> _orderyExpression = null;
        bool _orderDesc = false;
        int _timeOut = 30;
        System.Timers.Timer _timer = new System.Timers.Timer();

      

        public EntityTempCache()
        {
            StartTimer();
        }
        public EntityTempCache(int min)
        {
            _timeOut = min;
            StartTimer();
        }

        string defaultDB
        {
            get { return DBFactory.Instance.DefaultDBID(); }
        }
        private List<T> Load(string dbid = null)
        {
            using (DBContext context = new DBContext(dbid))
            {

                List<T> tResult = null;

                var query = context.Query<T>();
                if (_loadExpression != null)
                {
                    query = query.Where(_loadExpression);
                }

                if (_orderyExpression != null)
                {
                    if (!_orderDesc)
                    {
                        query = query.OrderBy(_orderyExpression);
                    }
                    else
                    {
                        query = query.OrderByDesc(_orderyExpression);
                    }
                }
                tResult = query.QueryAll().ToList();


                string dbkey = string.IsNullOrEmpty(dbid) ? defaultDB : dbid;
                var timeKey = new TimeKey() { Key = dbkey, LastTime = DateTime.Now };
                _dts.AddOrUpdate(timeKey, tResult, (oldkey, oldvalue) => tResult);
                SqlLogUtil.SendLog(LogType.Msg, $"加载 {timeKey.Key}[{timeKey.LastTime}]({nea.TableName})临时缓存({_timeOut}min) 共{tResult.Count}条");
                return tResult;
            }
        }

        private void StartTimer()
        {
            _timer.Interval = 60000;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }

        /// <summary>
        /// 超时删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var exprieKeys = _dts.Where(x => x.Key.LastTime < DateTime.Now.AddMinutes(-_timeOut)).Select(x => x.Key);
            foreach (var exprieKey in exprieKeys)
            {
                SqlLogUtil.SendLog(LogType.Msg, $"{exprieKey.Key}[{exprieKey.LastTime}]({nea.TableName})缓存超时 清理数据");
                _dts.TryRemove(exprieKey, out var removeData);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="expression">加载条件</param>
        /// <param name="min">数据缓存时常(分钟) 默认30</param>
        public void Init(Expression<Func<T, bool>> expression, int min, Expression<Func<T, object>> orderyExpression = null, bool desc = false)
        {
            _loadExpression = expression;
            _timeOut = min;
            _orderyExpression = orderyExpression;
            _orderDesc = desc;
            StartTimer();
        }
        public void Init(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderyExpression = null, bool desc = false)
        {
            _loadExpression = expression;
            _orderyExpression = orderyExpression;
            _orderDesc = desc;
            StartTimer();
        }
        public List<T> QueryAll(string dbid = null)
        {
            List<T> ts = null;
            string dbkey = string.IsNullOrEmpty(dbid) ? defaultDB : dbid;
            var timeKey = _dts.Keys.FirstOrDefault(x => x.Key == dbkey);
            if (timeKey != null)
            {
                if (!_dts.TryGetValue(timeKey, out ts))
                {
                    ts = Load(dbid);
                }
                else
                {
                    timeKey.LastTime = DateTime.Now;
                }
            }
            else
            {
                ts = Load(dbid);
            }
            return ts;
        }
        public List<T> Query(Expression<Func<T, bool>> expression, string dbid = null)
        {
            return QueryAll(dbid).Where(expression.Compile()).ToList();
        }

        public T QueryFirst(Expression<Func<T, bool>> expression = null, Expression<Func<T, object>> orderyExpression = null, bool orderByDesc = false, string dbid = null)
        {
            var query = QueryAll(dbid) as IEnumerable<T>;
            if (expression != null)
            {
                query = query.Where(expression.Compile());

            }
            if (orderyExpression != null)
            {
                if (!orderByDesc)
                {
                    query = query.OrderBy(orderyExpression.Compile());
                }
                else
                {
                    query = query.OrderByDescending(orderyExpression.Compile());
                }
            }
            return query.FirstOrDefault();
        }
        public PageResult<T> QueryPages(int pageNum, int pageSize, Expression<Func<T, bool>> expression = null, Expression<Func<T, object>> orderyExpression = null, bool orderByDesc = false, string dbid = null)
        {
            PageResult<T> pageResult = new PageResult<T>();
            var ds = QueryAll(dbid);

            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.ArrayData = ds;
            if (expression != null)
            {
                pageResult.ArrayData = pageResult.ArrayData.Where(expression.Compile());
            }
            pageResult.TotalCount = pageResult.ArrayData.Count();
            if (orderyExpression != null)
            {
                if (orderByDesc)
                {
                    pageResult.ArrayData = pageResult.ArrayData.OrderByDescending(orderyExpression.Compile());
                }
                else
                {
                    pageResult.ArrayData = pageResult.ArrayData.OrderBy(orderyExpression.Compile());
                }
            }

            pageResult.ArrayData = pageResult.ArrayData.Skip((pageNum - 1) * pageSize).Take(pageSize);
            return pageResult;
        }



        public bool Add(T t, string dbid = null)
        {
            var ds = QueryAll(dbid);
            using (DBContext context = new DBContext(dbid))
            {
                int result = context.Create(t);
                if (result > 0)
                {
                    if (!string.IsNullOrEmpty(nea.Auto_Increment))
                    {
                        t.SetValue(nea.Auto_Increment, result);
                    }

                    lock (ds)
                    {

                        //判断新增的是否符合初始化的时候设置的限定条件
                        ds.AddRange(ExpressionCheck(t));
                        ds.DoSort(_orderyExpression, _orderDesc);
                    }
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }
        public bool AddRange(IEnumerable<T> ts, string dbid = null)
        {
            var ds = QueryAll(dbid);
            using (DBContext context = new DBContext(dbid))
            {
                if (!string.IsNullOrEmpty(nea.Auto_Increment))
                {
                    foreach (var t in ts)
                    {
                        if (!Add(t, dbid))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    if (context.BatchCreate(ts) > 0)
                    {
                        lock (ds)
                        {
                            ds.AddRange(ExpressionCheck(ts.ToArray()));
                            ds.DoSort(_orderyExpression, _orderDesc);
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public List<T> ReLoad(string dbid = null)
        {
            string dbkey = string.IsNullOrEmpty(dbid) ? defaultDB : dbid;
            var timeKey = _dts.Keys.FirstOrDefault(x => x.Key == dbkey);
            if (timeKey != null)
            {
                _dts.TryRemove(timeKey, out var removeData);
            }
            return QueryAll(dbid);

        }



        public bool RefreshItem(T t, string dbid = null)
        {
            var ds = QueryAll(dbid);
            lock (ds)
            {
                //这地方可能有隐患
                ds.Remove(t);
                t.Refresh();
                ds.AddRange(ExpressionCheck(t));
                ds.DoSort(_orderyExpression, _orderDesc);
                return true;
            }
        }

        public bool RefreshItems(Expression<Func<T, bool>> expression, string dbid = null)
        {
            var ds = QueryAll(dbid);
            lock (ds)
            {
                ds.RemoveAll(x => expression.Compile().Invoke(x));
                ds.AddRange(ExpressionCheck(EntityFactory<T>.Query(expression, dbid).ToArray()));
                ds.DoSort(_orderyExpression, _orderDesc);
                return true;
            }
        }

        public int Remove(T t, string dbid = null)
        {
            var ds = QueryAll(dbid);
            lock (ds)
            {
                //这地方可能有隐患

                var sqlp = t.GetDelByPrimaryKey_SQL(string.IsNullOrEmpty(dbid) ? defaultDB : dbid);
                if (DBFactory.Instance.Execute(dbid, sqlp.SqlStr, sqlp.Params) > 0)
                {
                    ds.Remove(t);
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public int Remove(Expression<Func<T, bool>> expression, string dbid = null)
        {
            var ds = QueryAll(dbid);
            lock (ds)
            {
                var delds = ds.Where(expression.Compile());

                using (DBContext context = new DBContext(dbid))
                {
                    if (context.Delete(expression) > 0)
                    {
                        ds.RemoveAll(x => expression.Compile().Invoke(x));
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        public int Update(Expression<Func<T, T>> expressionNew, Expression<Func<T, bool>> expressionWhere, string dbid = null)
        {

            var ds = QueryAll(dbid);
            lock (ds)
            {
                using (DBContext context = new DBContext(dbid))
                {
                    if (context.Update(expressionNew, expressionWhere) > 0)
                    {
                        ds.RemoveAll(x => expressionWhere.Compile().Invoke(x));
                        ds.AddRange(ExpressionCheck(context.Query<T>().QueryAll(expressionWhere).ToArray()));
                        ds.DoSort(_orderyExpression, _orderDesc);
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }


        private IEnumerable<T> ExpressionCheck(params T[] ts)
        {
            if (_loadExpression != null)
            {
                return ts.Where(_loadExpression.Compile());
            }
            else
            {
                return ts;
            }
        }
        

    }

    internal class TimeKey
    {
        internal string Key { set; get; }
        internal DateTime LastTime { set; get; }
    }
}
