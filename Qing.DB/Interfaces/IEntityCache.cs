using Qing.DB;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Interfaces
{
    interface IEntityCache<T>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="expression"></param>
        void Init(Expression<Func<T, bool>> expression,Expression<Func<T, object>> orderyExpression = null, bool desc = false);
        /// <summary>
        /// 重新载入
        /// </summary>
        /// <param name="dbid"></param>
        /// <returns></returns>
        List<T> ReLoad(string dbid = null);
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="dbid"></param>
        /// <returns></returns>
        List<T> Query(Expression<Func<T, bool>> expression, string dbid = null);
        /// <summary>
        /// 条件排序查询第一个
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderyExpression"></param>
        /// <param name="orderByDesc"></param>
        /// <param name="dbid"></param>
        /// <returns></returns>
        T QueryFirst(Expression<Func<T, bool>> expression=null, Expression<Func<T, object>> orderyExpression = null, bool orderByDesc = false, string dbid = null);
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <param name="dbid"></param>
        /// <returns></returns>
        List<T> QueryAll(string dbid = null);

        PageResult<T> QueryPages(int pageNum, int pageSize, Expression<Func<T, bool>> expression=null, Expression<Func<T, object>> orderyExpression = null, bool orderByDesc = false, string dbid = null);


        bool Add(T t, string dbid = null);
        bool AddRange(IEnumerable<T> ts, string dbid = null);

        int Remove(T t, string dbid = null);
        int Remove(Expression<Func<T, bool>> expression, string dbid = null);

        int Update(Expression<Func<T, T>> expressionNew, Expression<Func<T, bool>> expressionWhere, string dbid = null);

        bool RefreshItems(Expression<Func<T, bool>> expression, string dbid = null);
        bool RefreshItem(T t, string dbid = null);
    }
}
