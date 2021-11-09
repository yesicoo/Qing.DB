using Dapper;
using Qing.DB.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Qing.DB
{
    public class DBFactory
    {
        public static readonly DBFactory Instance = new DBFactory();
        string defaultDBID = string.Empty;
        public bool RecordSqlTime = false;
        System.Timers.Timer t_ClearConn = new System.Timers.Timer(1000 * 60 * 5);

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnCount = 500;
        /// <summary>
        /// 当前连接数
        /// </summary>
        public int ConnCount { get { return DBConnPools.ConnCount; } }

        internal string DBType { get; set; }
        public bool AutoCreateTable { get; set; } = false;
        public int BatchCreateSplitCount { get; set; } = 1000;

        /// <summary>
        /// 连接空闲时间（之后被清理）
        /// </summary>
        public static int ClearSpan = 15;

        static object _lock_write = new object();
        bool isRun = false;
        public DBFactory()
        {

            t_ClearConn.Elapsed += (o, e) =>
            {
                lock (_lock_write)
                {
                    if (isRun) { return; }
                    isRun = true;
                }

                try
                {
                    var ConnCount = DBConnPools.ConnCount;
                    DBConnPools.DisposeUnAvailableConn();

                    SqlLogUtil.SendLog(LogType.Msg, $"原数量:{ConnCount};释放后:{DBConnPools.ConnCount};忙碌:{DBConnPools.BusyCount};空闲:{DBConnPools.FreeCount}");
                }
                catch (Exception ex)
                {
                    SqlLogUtil.SendLog(LogType.Error, "连接池回收失败:" + ex.ToString());
                }
                finally
                {
                    lock (_lock_write)
                        isRun = false;
                }
            };
            t_ClearConn.Start();

        }
        /// <summary>
        /// 返回默认数据库ID
        /// </summary>
        /// <returns></returns>
        public string DefaultDBID()
        {
            return defaultDBID;
        }
        public string GetConnStr(string tag)
        {
            var info = DBConnPools.GetConnInfo(tag);
            return info?.DBConnStr;
        }
        public string GetConnStr()
        {
            return GetConnStr("Default");
        }

        /// <summary>
        /// 获取链接对象
        /// </summary>
        /// <param name="dbid"></param>
        /// <returns></returns>
        public DBConnectionItem GetDBConnectionItem(string dbid = null, string tag = null)
        {
            return DBConnPools.GetConnectionByDBID(dbid, tag);
        }
        /// <summary>
        /// 创建一个新的连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public DbConnection CreateSqlConnection(string connStr)
        {
            if (DBType == "MsSql")
            {
                return new SqlConnection(connStr);
            }
            else
            {
                return new MySqlConnection(connStr);
            }
        }

        #region 初始化
        /// <summary>
        /// 添加或编辑链接
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="dbType">MySql/MsSql(SqlServer)</param>
        //public void SetConnStr(string connStr,string dbType="MySql")
        //{
        //    this.DBType = dbType;
        //    if (dbType == "MsSql")
        //    {
        //        nea._Lr = "[";
        //        nea._Rr = "]";
        //        defaultDBID = connStr.Split(';').FirstOrDefault(x => x.ToLower().Contains("initial catalog"))?.Split('=')?[1]?.Trim();
        //    }
        //    else
        //    {
        //        defaultDBID = connStr.Split(';').FirstOrDefault(x => x.ToLower().Contains("database"))?.Split('=')?[1]?.Trim();
        //    }
        //    SqlLogUtil.SendLog(LogType.Msg, "DefaultDB:" + defaultDBID);
        //    DBConnPool.SetConnStr(connStr);
        //}

        public void SetConnStr(string connStr, string dbType = "MySql")
        {
            SetConnStr("Default", connStr, dbType);
        }
        public void SetConnStr(string tag, string connStr, string dbType = "MySql")
        {
            DBConnPools.SetConnStr(tag, connStr, dbType);
            ThreadPool.SetMinThreads(64, 64);
        }

        public bool SetMinThreads(int count)
        {
           return ThreadPool.SetMinThreads(count, count);
        }




        #endregion

      

        #region 执行sql操作
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int Execute(string dbid, string sql, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            var tick = DateTime.Now.Ticks;
            try
            {
                return sqlConn.Execute(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

        public Task<int> ExecuteAsync(string dbid, string sql, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);
            try
            {
                return sqlConn.ExecuteAsync(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

        public int Execute(string dbid, string sql, DynamicParameters parames, string tag = null)
        {
           // var tick = DateTime.Now.Ticks;
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.Execute(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

        public Task<int> ExecuteAsync(string dbid, string sql, DynamicParameters parames, string tag = null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.ExecuteAsync(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

       
        public int Execute(IDbTransaction transaction, string sql)
        {

            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(sql))
                throw new Exception("Sql语句不可为空");

            var tick = DateTime.Now.Ticks;
            try
            {
                return transaction.Connection.Execute(sql, null, transaction);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql,(DateTime.Now.Ticks - tick) / 10000.00m);
            }

        }

        public Task<int> ExecuteAsync(IDbTransaction transaction, string sql)
        {

            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(sql))
                throw new Exception("Sql语句不可为空");

            var tick = DateTime.Now.Ticks;
            try
            {
                return transaction.Connection.ExecuteAsync(sql, null, transaction);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }

        }

        public int Execute(IDbTransaction transaction, string sql, DynamicParameters parames)
        {

            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(sql))
                throw new Exception("Sql语句不可为空");

            var tick = DateTime.Now.Ticks;
            try
            {
                return transaction.Connection.Execute(sql, parames, transaction);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }

        }

        public Task<int> ExecuteAsync(IDbTransaction transaction, string sql, DynamicParameters parames)
        {

            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(sql))
                throw new Exception("Sql语句不可为空");

            var tick = DateTime.Now.Ticks;
            try
            {
                return transaction.Connection.ExecuteAsync(sql, parames, transaction);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }

        }
        public int ExecuteProcedure(string dbid, string procedureName, DynamicParameters parames, string tag = null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.ExecuteProcedure(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }
        public Task<int> ExecuteProcedureAsync(string dbid, string procedureName, DynamicParameters parames,string tag=null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.ExecuteProcedureAsync(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }
        public int ExecuteProcedure(IDbTransaction transaction, string procedureName, DynamicParameters parames)
        {

            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;
            try
            {
                return transaction.Connection.Execute(procedureName, parames,commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }

          
        }
        public Task<int> ExecuteProcedureAsync(IDbTransaction transaction, string procedureName, DynamicParameters parames)
        {
            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;

            try
            {
                return transaction.Connection.ExecuteAsync(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T QuerySingleByProcedure<T>(string dbid,string procedureName, DynamicParameters parames,string tag=null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QuerySingleByProcedure<T>(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }
        public Task<T> QuerySingleByProcedureAsync<T>(string dbid, string procedureName, DynamicParameters parames, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QuerySingleByProcedureAsync<T>(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

        public T QuerySingleByProcedure<T>(IDbTransaction transaction,  string procedureName, DynamicParameters parames)
        {
            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;

            try
            {
                return transaction.Connection.QueryFirstOrDefault<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> QuerySingleByProcedureAsync<T>(IDbTransaction transaction, string procedureName, DynamicParameters parames)
        {
            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;

            try
            {
                return transaction.Connection.QueryFirstOrDefaultAsync<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public IEnumerable<T> QueryByProcedure<T>(string dbid, string procedureName, DynamicParameters parames, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryByProcedure<T>(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }
        public Task<IEnumerable<T>> QueryByProcedureAsync<T>(string dbid, string procedureName, DynamicParameters parames,string tag=null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryByProcedureAsync<T>(procedureName, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }
        }

        public IEnumerable<T> QueryByProcedure<T>(IDbTransaction transaction, string procedureName, DynamicParameters parames)
        {
            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;

            try
            {
                return transaction.Connection.Query<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<IEnumerable<T>> QueryByProcedureAsync<T>(IDbTransaction transaction, string procedureName, DynamicParameters parames)
        {
            if (transaction == null)
                throw new Exception("事务状态为null");
            if (string.IsNullOrEmpty(procedureName))
                throw new Exception("存储过程名称不可为空");

            var tick = DateTime.Now.Ticks;

            try
            {
                return transaction.Connection.QueryAsync<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public T QueryFirstOrDefault<T>(string dbid, string sql, DynamicParameters parames,string tag=null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            var tick = DateTime.Now.Ticks;
            try
            {
                return sqlConn.QueryFirst<T>(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string dbid, string sql, DynamicParameters parames, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryFirstAsync<T>(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        public T QueryFirstOrDefault<T>(string dbid, string sql,string tag=null)
        {
          //  dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid, tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryFirst<T>(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }
        public Task<T> QueryFirstOrDefaultAsync<T>(string dbid, string sql, string tag = null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryFirstAsync<T>(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        public IEnumerable<T> Query<T>(string dbid, string sql, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.Query<T>(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }
        public Task<IEnumerable<T>> QueryAsync<T>(string dbid, string sql, string tag = null)
        {
           // dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryAsync<T>(sql);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        public IEnumerable<T> Query<T>(string dbid, string sql, DynamicParameters parames, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            var tick = DateTime.Now.Ticks;
            try
            {
                return sqlConn.Query<T>(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        public Task<IEnumerable<T>> QueryAsync<T>(string dbid, string sql, DynamicParameters parames, string tag = null)
        {
            //dbid = string.IsNullOrEmpty(dbid) ? defaultDBID : dbid;
            var sqlConn = DBConnPools.GetConnectionByDBID(dbid,tag);
            if (sqlConn == null)
                throw new Exception("获取连接对象错误：" + dbid);

            try
            {
                return sqlConn.QueryAsync<T>(sql, parames);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConn.GiveBack();
            }

        }

        #endregion

      
        public void Dispose()
        {
            DBConnPools.Dispose();
        }

    }
}
