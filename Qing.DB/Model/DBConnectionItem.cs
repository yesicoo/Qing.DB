using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;
using Qing.DB.Model;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Qing.DB.Model
{
    public class DBConnectionItem
    {
        public DbConnection SqlConnection { set; get; }

        public DbTransaction transaction { set; get; } = null;
        public bool UseTransaction { set; get; }
        public bool Available { set; get; }
        public DateTime SleepTime { set; get; } = DateTime.MaxValue;
        internal DBConnInfo ConnInfo { set; get; }
        internal string Tag { set; get; }


        public DBConnectionItem(DBConnInfo connInfo)
        {
            ConnInfo = connInfo;

            if (connInfo.DBType == "MsSql")
            {
                SqlConnection = new SqlConnection(connInfo.DBConnStr);
            }
            else
            {
                SqlConnection = new MySqlConnection(connInfo.DBConnStr);
            }
        }
        public void ConnectionOpen()
        {
            if (SqlConnection.State != ConnectionState.Open)
                SqlConnection.Open();
        }

        public void GiveBack()
        {
            this.UseTransaction = false;
            this.Available = true;
            this.SleepTime = DateTime.Now;
            // Qing.QLog.SendLog_Debug(DBID);
        }

        #region 操作
        public int DBVer
        {
            get
            {
                if (ConnInfo.DBType == "MsSql")
                {
                    return int.Parse(SqlConnection.ServerVersion.Split('.')[0]);
                }
                else
                {
                    return 0;
                }
            }
        }


        public bool DBConnectionCheck()
        {
            try
            {

                if (ConnInfo.DBType == "MsSql")
                {
                    try
                    {
                        var result = SqlConnection.Execute("SELECT 1;");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        SqlLogUtil.SendLog(LogType.Msg, ex.Message);
                        return false;
                    }
                }
                else
                {
                    return ((MySqlConnection)SqlConnection).Ping();
                }
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Msg, "数据库链接活性检测失败:" + ex.Message);
                return false;
            }
        }

        #region 操作

        public DbTransaction CreateTransaction()
        {
            transaction = SqlConnection.BeginTransaction();
            this.UseTransaction = true;
            return transaction;
        }

        internal void TransactionDispose()
        {
            if (UseTransaction)
            {
                this.UseTransaction = false;
                transaction = null;
            }
        }

        internal bool PingTest()
        {
            if (ConnInfo.DBType == "MsSql")
            {
                try
                {
                    return SqlConnection.Execute("select 1;") > 0;
                }
                catch (Exception ex)
                {
                    SqlLogUtil.SendLog(LogType.Error, ex.Message);
                    return false;
                }
            }
            else
            {
                return ((MySql.Data.MySqlClient.MySqlConnection)SqlConnection).Ping();
            }
        }


        public DbTransaction CreateTransaction(IsolationLevel iso)
        {
            transaction = SqlConnection.BeginTransaction(iso);
            this.UseTransaction = true;
            return transaction;
        }

        public IEnumerable<T> Query<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Query<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("Query Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryAsync<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public IEnumerable<T> Query<T>(string sql, DynamicParameters sqlParams)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Query<T>(sql, sqlParams, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Query Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters sqlParams)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryAsync<T>(sql, sqlParams, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T QueryFirst<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefault<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryFirstOrDefault Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> QueryFirstAsync<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefaultAsync<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryFirstOrDefaultAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T QueryFirst<T>(string sql, DynamicParameters sqlParams)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefault<T>(sql, sqlParams, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryFirstOrDefault Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> QueryFirstAsync<T>(string sql, DynamicParameters sqlParams)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefaultAsync<T>(sql, sqlParams, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QueryFirstOrDefaultAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T QuerySingle<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {

                return SqlConnection.QuerySingleOrDefault<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QuerySingleOrDefault Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

       

        public Task<T> QuerySingleAsync<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QuerySingleOrDefaultAsync<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T QuerySingle<T>(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QuerySingleOrDefault<T>(sql, parames, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> QuerySingleAsync<T>(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QuerySingleOrDefaultAsync<T>(sql, parames, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("QuerySingleOrDefaultAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T ExecuteScalar<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteScalar<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> ExecuteScalarAsync<T>(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteScalarAsync<T>(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("ExecuteScalarAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public T ExecuteScalar<T>(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteScalar<T>(sql, parames, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteScalarAsync<T>(sql, parames, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("ExecuteScalarAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public int Execute(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Execute(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<int> ExecuteAsync(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteAsync(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("ExecuteAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public int Execute(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Execute(sql, parames, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<int> ExecuteAsync(string sql, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteAsync(sql, parames, transaction);
            }
            catch (Exception ex)
            {
                throw new Exception("ExecuteAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public int ExecuteProcedure(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Execute(procedureName, parames, transaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<int> ExecuteProcedureAsync(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteAsync(procedureName, parames, transaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public T QuerySingleByProcedure<T>(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefault<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<T> QuerySingleByProcedureAsync<T>(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryFirstOrDefaultAsync<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public IEnumerable<T> QueryByProcedure<T>(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.Query<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        public Task<IEnumerable<T>> QueryByProcedureAsync<T>(string procedureName, DynamicParameters parames)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.QueryAsync<T>(procedureName, parames, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Execute Error:" + procedureName, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, procedureName, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public IDataReader ExecuteReader(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteReader(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("Execute Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }

        public Task<DbDataReader> ExecuteReaderAsync(string sql)
        {
            var tick = DateTime.Now.Ticks;
            try
            {
                return SqlConnection.ExecuteReaderAsync(sql, null, transaction);
            }
            catch (Exception ex)
            {

                throw new Exception("ExecuteReaderAsync Error:" + sql, ex);
            }
            finally
            {
                SqlLogUtil.SendLog(LogType.SQL, sql, (DateTime.Now.Ticks - tick) / 10000.00m);
            }
        }
        #endregion

        internal int GetLastIdentity()
        {
            if (ConnInfo.DBType == "MsSql")
            {
                return SqlConnection.ExecuteScalar<int>("SELECT SCOPE_IDENTITY();");
            }
            else
            {
                return SqlConnection.ExecuteScalar<int>("SELECT LAST_INSERT_ID();"); ;
            }
        }

    }
    #endregion
}
