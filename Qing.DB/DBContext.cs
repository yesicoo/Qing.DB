using Dapper;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    public class DBContext : IDisposable
    {
        DBConnectionItem connectionItem;
        DbTransaction dbTransaction;
        string DBName = string.Empty;
        public string Tag = string.Empty;
        public string CurrentDB
        {
            get
            {
                if (connectionItem == null)
                {
                    connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
                }
                else
                {

                    if (connectionItem.SqlConnection.Database != DBName)
                    {
                        connectionItem.SqlConnection.ChangeDatabase(DBName);
                    }
                }

                return connectionItem.SqlConnection.Database;
            }
        }

        public DBContext(string dbName = null, string tag = null)
        {
            Tag = tag;
            DBName = string.IsNullOrEmpty(dbName) ? DBConnPools.DefaultDBID(tag) : dbName;

            if (connectionItem != null && connectionItem.SqlConnection.Database != DBName)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, tag);
            }
        }

        //public DBContext()
        //{
        //    DBName = DBFactory.Instance.DefaultDBID();
        //    if (connectionItem != null && connectionItem.SqlConnection.Database != DBName)
        //    {
        //        connectionItem = DBConnPools.GetConnectionByDBID(DBName);
        //    }
        //}

        public void ChangeRuningDB(string dbid)
        {
            DBName = dbid;
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            else
            {
                if (connectionItem.SqlConnection.Database != DBName)
                {
                    connectionItem.SqlConnection.ChangeDatabase(DBName);
                }
            }
        }



        #region 事务
        public DbTransaction BeginTransaction()
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            dbTransaction = connectionItem.CreateTransaction();
            return dbTransaction;
        }
        public DbTransaction BeginTransaction(IsolationLevel iso)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            dbTransaction = connectionItem.CreateTransaction(iso);
            return dbTransaction;
        }

        public void Commit()
        {
            try
            {
                if (dbTransaction != null)
                {
                    dbTransaction.Commit();
                    dbTransaction = null;
                    connectionItem.TransactionDispose();
                }
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Error, ex.ToString());
                Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
                dbTransaction = null;
                connectionItem.TransactionDispose();
            }
        }
        #endregion


        /// <summary>
        /// 创建查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="AliasModel">别名模式</param>
        /// <returns></returns>
        public IQingQuery<T> Query<T>(string tableSuffix = null, bool AliasModel = false) where T : BaseEntity
        {

            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return new QingQuery<T>(connectionItem, AliasModel, tableSuffix);

        }


        /// <summary>
        /// 反解析SQL查询条件到Query对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SqlCondition"></param>
        /// <param name="AliasModel"></param>
        /// <returns></returns>
        public IQingQuery<T> CreateQuery<T>(SqlFilter fc, bool AliasModel = false, string tableSuffix = null) where T : BaseEntity
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            IQingQuery<T> query = new QingQuery<T>(connectionItem, AliasModel, tableSuffix);
            return query.ResolveFilterCondition(fc);

        }

        public IEnumerable<T> QueryData<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            return connectionItem.Query<T>(sql);

        }

        public async Task<IEnumerable<T>> QueryDataAsync<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            return await connectionItem.QueryAsync<T>(sql);

        }

        public T New<T>() where T : BaseEntity, new()
        {
            return new T();
        }
        public Task<int> CreateAsync<T>(T t, string tableSuffix = null, bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
            {
                return Task.FromResult(-1);
            }
            return Task.Run(() => { return Create(t, tableSuffix, ignoreInc); });
        }
        public int Create<T>(T t, string tableSuffix = null, bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
            {
                return -1;
            }
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            var ps = t.GetInsert_Sql(connectionItem.SqlConnection.Database, tableSuffix, ignoreInc, Tag);
            int sqlResult = -1;
            try
            {
                sqlResult = connectionItem.Execute(ps.SqlStr, ps.Params);
            }
            catch (Exception ex)
            {
                if (DBFactory.Instance.AutoCreateTable)
                {

                    if ((ex.InnerException.Message.StartsWith("对象名") && ex.InnerException.Message.EndsWith("无效。")) || ex.InnerException.Message.StartsWith("Invalid object name"))
                    {
                        TableWatcher.CreateTable<T>(DBName, tableSuffix, tag: Tag);
                        sqlResult = connectionItem.Execute(ps.SqlStr, ps.Params);
                        return sqlResult;
                    }
                }
                throw new Exception(ex.Message);
            }


            var ab = t.GetNEA(Tag);
            if (!string.IsNullOrEmpty(ab.Auto_Increment))
            {
                return connectionItem.GetLastIdentity();// connectionItem.ExecuteScalar<int>("SELECT LAST_INSERT_ID();");
            }
            else
            {
                return sqlResult;
            }

        }


        public int BatchCreate<T>(IEnumerable<T> ts, string tableSuffix = null, bool ignoreInc = false) where T : BaseEntity
        {
            if (ts == null || ts.Count() == 0)
            {
                throw new Exception("对象集合不可为空");
            }
            else
            {
                if (connectionItem == null)
                {
                    connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
                }

                var datas = ts.ToList();
                int splitCount = DBFactory.Instance.BatchCreateSplitCount;
                if (datas.Count > splitCount)
                {
                    int sum = 0;
                    for (int i = 0; i < datas.Count; i += splitCount)
                    {
                        int result = BatchCreate(datas.Skip(i).Take(splitCount), tableSuffix, ignoreInc);
                        if (result < 0)
                        {
                            return -1;
                        }
                        else
                        {
                            sum += result;
                        }
                    }
                    return sum;
                }
                else
                {
                    var ps = datas.GetInsert_Sql(connectionItem.SqlConnection.Database, tableSuffix, ignoreInc, Tag);
                    try
                    {
                        return connectionItem.Execute(ps.SqlStr, ps.Params);
                    }
                    catch (Exception ex)
                    {
                        if (DBFactory.Instance.AutoCreateTable)
                        {

                            if ((ex.InnerException.Message.StartsWith("对象名") && ex.InnerException.Message.EndsWith("无效。")) || ex.InnerException.Message.StartsWith("Invalid object name"))
                            {
                                TableWatcher.CreateTable<T>(DBName, tableSuffix, ignoreInc, Tag);
                                return connectionItem.Execute(ps.SqlStr, ps.Params);
                            }

                        }
                        throw new Exception (ex.Message);
                    }
                }

                //var ps = ts.ToList().GetInsert_Sql(connectionItem.SqlConnection.Database, ignoreInc);
                //return connectionItem.Execute(ps.SqlStr, ps.Params);
            }
        }

        public Task<int> BatchCreateAsync<T>(IEnumerable<T> ts, string tableSuffix = null, bool ignoreInc = false) where T : BaseEntity
        {
            if (ts == null || ts.Count() == 0)
            {
                throw new Exception("对象集合不可为空");
            }
            else
            {
                return Task.FromResult(BatchCreate(ts, tableSuffix, ignoreInc));
                // return Task.Run(()=> { return BatchCreate(ts, tableSuffix, ignoreInc); });
            }
        }

        public int Delete<T>(Expression<Func<T, bool>> expression, string tableSuffix = null) where T : BaseEntity
        {
            return Query<T>(tableSuffix).Remove(expression);
        }

        public Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression, string tableSuffix = null) where T : BaseEntity
        {
            return Query<T>(tableSuffix).RemoveAsync(expression);
        }

        public int DeleteByPrimaryKeys<T>(string[] primaryKeys, string tableSuffix = null) where T : BaseEntity
        {
            return Query<T>(tableSuffix).RemoveByPrimaryKeys(primaryKeys);
        }

        public Task<int> DeleteByPrimaryKeysAsync<T>(string[] primaryKeys, string tableSuffix = null) where T : BaseEntity
        {
            return Query<T>(tableSuffix).RemoveByPrimaryKeysAsync(primaryKeys);
        }
        public int Update<T>(Expression<Func<T, T>> expressionNew, Expression<Func<T, bool>> expressionWhere, string tableSuffix = null) where T : BaseEntity
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            var nea = Tools.GetNEA(typeof(T), Tag);// Tools.GetNEA(typeof(T));

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew, Tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(expressionWhere.Body, tag: Tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";

            return connectionItem.Execute(sql, ps.Params);

        }

        public Task<int> UpdateAsync<T>(Expression<Func<T, T>> expressionNew, Expression<Func<T, bool>> expressionWhere, string tableSuffix = null, string tag = null) where T : BaseEntity
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }

            var nea = Tools.GetNEA(typeof(T), Tag);// Tools.GetNEA(typeof(T));

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew, Tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(expressionWhere.Body, tag: Tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";

            return connectionItem.ExecuteAsync(sql, ps.Params);

        }
        public int UpdateByPrimaryKeys<T>(Expression<Func<T, T>> expressionNew, string[] primaryKeys, string tableSuffix = null, string tag = null) where T : BaseEntity
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, tag);
            }
            ParamSql ps = new ParamSql();
            var nea = Tools.GetNEA(typeof(T), tag);//Tools.GetNEA(typeof(T));
            var pks = nea.PrimaryKey.Split(',').ToList();
            if (pks.Count != primaryKeys.Length)
            {
                SqlLogUtil.SendLog(LogType.Error, "DeleteByPrimaryKeys:主键参数数量不一致");
                return -1;
            }
            else
            {
                var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew, tag);
                StringBuilder sb_setStr = new StringBuilder();
                foreach (var item in dic_update)
                {
                    sb_setStr.Append($",{item.SqlStr}");
                    ps.Params.AddDynamicParams(item.Params);
                }
                string setStr = sb_setStr.ToString().TrimStart(',');


                List<string> wheres = new List<string>();
                for (int i = 0; i < pks.Count; i++)
                {
                    var key = Tools.RandomCode(6, 0, true);
                    wheres.Add($"{nea._Lr}{pks[i]}{nea._Rr}=@{key}");
                    ps.Params.Add(key, primaryKeys[i]);
                }

                StringBuilder sb_sql = new StringBuilder($"update  {connectionItem.SqlConnection.Database}{nea._Rr}.{nea.TableSuffixName(tableSuffix)} set {setStr} where {string.Join(" and ", wheres)};");
                string sql = sb_sql.ToString();

                return connectionItem.Execute(sql, ps.Params);

            }
        }

        public Task<int> UpdateByPrimaryKeysAsync<T>(Expression<Func<T, T>> expressionNew, string[] primaryKeys, string tableSuffix = null, string tag = null) where T : BaseEntity
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            ParamSql ps = new ParamSql();
            var nea = Tools.GetNEA(typeof(T), Tag);// Tools.GetNEA(typeof(T));
            var pks = nea.PrimaryKey.Split(',').ToList();
            if (pks.Count != primaryKeys.Length)
            {
                SqlLogUtil.SendLog(LogType.Error, "DeleteByPrimaryKeys:主键参数数量不一致");
                return Task.FromResult(-1);
            }
            else
            {
                var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew, Tag);
                StringBuilder sb_setStr = new StringBuilder();
                foreach (var item in dic_update)
                {
                    sb_setStr.Append($",{item.SqlStr}");
                    ps.Params.AddDynamicParams(item.Params);
                }
                string setStr = sb_setStr.ToString().TrimStart(',');


                List<string> wheres = new List<string>();
                for (int i = 0; i < pks.Count; i++)
                {
                    var key = Tools.RandomCode(6, 0, true);
                    wheres.Add($"{nea._Lr}{pks[i]}{nea._Rr}=@{key}");
                    ps.Params.Add(key, primaryKeys[i]);
                }

                StringBuilder sb_sql = new StringBuilder($"update  {nea._Lr}{connectionItem.SqlConnection.Database}{nea._Lr}.{nea.TableSuffixName(tableSuffix)} set {setStr} where {string.Join(" and ", wheres)};");
                string sql = sb_sql.ToString();

                return connectionItem.ExecuteAsync(sql, ps.Params);

            }
        }

        public int Execute(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.Execute(sql);
        }
        public Task<int> ExecuteAsync(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteAsync(sql);
        }

        public T ExecuteScalar<T>(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteScalar<T>(sql, parames);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteScalarAsync<T>(sql, parames);
        }

        public T ExecuteScalar<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteScalar<T>(sql);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteScalarAsync<T>(sql);
        }

        public int Execute(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.Execute(sql, parames);
        }
        public Task<int> ExecuteAsync(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteAsync(sql, parames);
        }
        public int ExecuteProcedure(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteProcedure(procedureName, parames);
        }
        public Task<int> ExecuteProcedureAsync(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteProcedureAsync(procedureName, parames);
        }
        public T QuerySingleByProcedure<T>(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QuerySingleByProcedure<T>(procedureName, parames);
        }

        public Task<T> QuerySingleByProcedureAsync<T>(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QuerySingleByProcedureAsync<T>(procedureName, parames);
        }

        public IEnumerable<T> QueryByProcedure<T>(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QueryByProcedure<T>(procedureName, parames);
        }

        public Task<IEnumerable<T>> QueryByProcedureAsync<T>(string procedureName, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QueryByProcedureAsync<T>(procedureName, parames);
        }
        public IEnumerable<T> ExecuteQuery<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.Query<T>(sql);
        }

        public Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QueryAsync<T>(sql);
        }

        public IEnumerable<T> ExecuteQuery<T>(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.Query<T>(sql, parames);
        }
        public Task<IEnumerable<T>> ExecuteQueryAsync<T>(string sql, DynamicParameters parames)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.QueryAsync<T>(sql, parames);
        }

        public IDataReader ExecuteReader(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteReader(sql);
        }
        public Task<DbDataReader> ExecuteReaderAsync(string sql)
        {
            if (connectionItem == null)
            {
                connectionItem = DBConnPools.GetConnectionByDBID(DBName, Tag);
            }
            return connectionItem.ExecuteReaderAsync(sql);
        }
        public bool CheckTableExist<T>(string tableSuffix = null)
        {
            var nea = Tools.GetNEA(typeof(T), Tag);
            var tableName = Tools.ConvertSuffixTableName(nea._tableName, tableSuffix, nea._dbType);
            if (nea._dbType == "MsSql")
            {
                string sql = $"if object_id('{tableName}') is not null print 1 else  print 0";
                return ExecuteScalar<int>(sql) == 1;
            }
            else
            {
                string sql = $"SELECT COUNT(*) FROM information_schema.TABLES WHERE table_name ='{tableName}';";
                return ExecuteScalar<int>(sql) == 1;
            }
        }

        public void Dispose()
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
                dbTransaction.Dispose();
            }
            if (connectionItem != null)
            {
                connectionItem.GiveBack();
            }
        }
    }
}
