using Dapper;
using Qing.DB.QingQuery;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    internal class QingQuery<T> : IQingQuery<T> where T : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { get; set; }
        internal string TableSuffix { set; get; }

        bool SelectAlias = false;


        #region 初始化
        public QingQuery(DBConnectionItem connection, bool selectAlias = false,string tableSuffix=null)
        {
            dbConnection = connection;
            SelectAlias = selectAlias;
            TableSuffix = tableSuffix;
        }

        #endregion

        /// <summary>
        /// 解析多条件自定义查询
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public IQingQuery<T> ResolveFilterCondition(SqlFilter fc)
        {

            var a = default(T).GetFieldsAlias();

            if (fc.OrWhere.Count > 0)
            {
                List<ParamSql> orSql = new List<ParamSql>();
                foreach (var item in fc.OrWhere)
                {
                    ParamSql ps = new ParamSql();
                    if (a.ContainsKey(item.FieldName))
                    {
                        var key = Tools.RandomCode(6, 0, true);
                        ps.SqlStr = $"({item.FieldName} {item.Condition} @{key})";
                        ps.Params.Add(key, item.Value);
                        orSql.Add(ps);
                    }
                }
                if (orSql.Count > 0)
                {
                    this.WhereStr += string.IsNullOrEmpty(this.WhereStr) ? "" : " and ";
                    List<string> sqlWhere = new List<string>();
                    orSql.ForEach(x =>
                    {
                        this.SqlParams.AddDynamicParams(x.Params);
                        sqlWhere.Add(x.SqlStr);
                    });
                    this.WhereStr += $"({string.Join(" or ", sqlWhere)})";
                }
            }

            if (fc.AndWhere.Count > 0)
            {
                foreach (var item in fc.AndWhere)
                {
                    if (a.ContainsKey(item.FieldName))
                    {
                        this.WhereStr += string.IsNullOrEmpty(this.WhereStr) ? "" : " and ";
                        var key = Tools.RandomCode(6, 0, true);
                        this.WhereStr += $"({item.FieldName} {item.Condition} @{key})";
                        this.SqlParams.Add(key, item.Value);
                    }
                }
            }

            if (fc.Orders.Count > 0)
            {
                foreach (var item in fc.Orders)
                {
                    if (a.ContainsKey(item.FieldName))
                    {
                        this.OrderByStr += $",{item.FieldName}{(item.Desc ? " Desc" : "")}";
                    }
                }
            }
            if (fc.PageNum > 0)
            {
                this.Limit = $" limit{(fc.PageNum - 1) * fc.PageSize},{fc.PageSize}";
            }

            return this;
        }

        /// <summary>
        /// 条件解析
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQingQuery<T> Where(Expression<Func<T, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }

        /// <summary>
        /// 条件查询返回结果
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryAll(Expression<Func<T, bool>> expression)
        {
            var eq = ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
            return QueryAll();
        }
        public async Task<IEnumerable<T>> QueryAllAsync(Expression<Func<T, bool>> expression)
        {
            var eq = ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
            return await QueryAllAsync();
        }

        /// <summary>
        /// 条件查询返回首条结果
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T QueryFirst(Expression<Func<T, bool>> expression)
        {
            ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
            return QueryFirst();
        }
        public async Task<T> QueryFirstAsync(Expression<Func<T, bool>> expression)
        {
            ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
            return await QueryFirstAsync();
        }

        public IQingQuery<T> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, SelectAlias, dbConnection.Tag);
            //string sql = BuildSql();
            //return dbConnection.Query<TResult>(sql, SqlParams);
        }
        /// <summary>
        /// 排除某些字段查询其余字段
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IQingQuery<T> SelectExcept<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ExpressionResolver.ResoveSelectExceptExpression(this, expression, SelectAlias,dbConnection.Tag);
        }

        public IQingQuery<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag:dbConnection.Tag);
        }

        public IQingQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, tag:dbConnection.Tag);
        }
        public IQingQuery<T> OrderByDesc<TResult>(Expression<Func<T, TResult>> expression)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, true,tag: dbConnection.Tag);
        }

        public IQingQuery<T> OrderBy(string fieldstr, bool desc = true)
        {
            string[] fields = fieldstr.Split(new char[] { ',', '，', '|' });
            foreach (var field in fields)
            {
                this.OrderByStr += $",{field}{(desc ? " Desc" : "")}";
            }
            return this;
        }

        public IEnumerable<Tresult> QueryAll<Tresult>()
        {
            string sql = BuildSql();
            return dbConnection.Query<Tresult>(sql, SqlParams);
        }
        public Task<IEnumerable<Tresult>> QueryAllAsync<Tresult>()
        {
            string sql = BuildSql();
            return dbConnection.QueryAsync<Tresult>(sql, SqlParams);
        }
        public IEnumerable<T> QueryAll()
        {
            string sql = BuildSql();
            return dbConnection.Query<T>(sql, SqlParams);
        }
        public Task<IEnumerable<T>> QueryAllAsync()
        {
            string sql = BuildSql();
            return dbConnection.QueryAsync<T>(sql, SqlParams);
        }
        public T QueryFirst()
        {
            string sql = BuildSql_Custom_LimitCount(1);
            return dbConnection.QueryFirst<T>(sql, SqlParams);
        }
        public Task<T> QueryFirstAsync()
        {
            string sql = BuildSql_Custom_LimitCount(1);
            return dbConnection.QueryFirstAsync<T>(sql, SqlParams);
        }
        public Tresult QueryFirst<Tresult>()
        {
            string sql = BuildSql_Custom_LimitCount(1);
            return dbConnection.QueryFirst<Tresult>(sql, SqlParams);
        }
        public Task<Tresult> QueryFirstAsync<Tresult>()
        {
            string sql = BuildSql_Custom_LimitCount(1);
            return dbConnection.QueryFirstAsync<Tresult>(sql, SqlParams);
        }
        public IEnumerable<T> QueryTop(int top = 1)
        {
            string sql = BuildSql_Custom_LimitCount(top);
            return dbConnection.Query<T>(sql, SqlParams);
        }
        public Task<IEnumerable<T>> QueryTopAsync(int top = 1)
        {
            string sql = BuildSql_Custom_LimitCount(top);
            return dbConnection.QueryAsync<T>(sql, SqlParams);
        }

        public IEnumerable<TResult> QueryTop<TResult>(int top = 1)
        {
            string sql = BuildSql_Custom_LimitCount(top);
            return dbConnection.Query<TResult>(sql, SqlParams);
        }
        public Task<IEnumerable<TResult>> QueryTopAsync<TResult>(int top = 1)
        {
            string sql = BuildSql_Custom_LimitCount(top);
            return dbConnection.QueryAsync<TResult>(sql, SqlParams);
        }

        public IEnumerable<T> QueryWithPage(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            // var limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            if (dbConnection.ConnInfo.DBType == "MsSql")
            {
                return dbConnection.Query<T>(BuildSql_Custom_LimitPageByMsSql(pageNum, pageSize, dbConnection.DBVer), SqlParams);
            }
            else
            {
                return dbConnection.Query<T>(BuildSql_Custom_LimitPageByMySql(pageNum, pageSize), SqlParams);
            }
            // string sql = BuildSql_Custom_LimitPage(pageNum, pageSize);

        }
        public Task<IEnumerable<T>> QueryWithPageAsync(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            //var limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            //string sql = BuildSql_Custom_Limit(limit);
            //return dbConnection.QueryAsync<T>(sql, SqlParams);
            if (dbConnection.ConnInfo.DBType == "MsSql")
            {
                return dbConnection.QueryAsync<T>(BuildSql_Custom_LimitPageByMsSql(pageNum, pageSize, dbConnection.DBVer), SqlParams);
            }
            else
            {
                return dbConnection.QueryAsync<T>(BuildSql_Custom_LimitPageByMySql(pageNum, pageSize), SqlParams);
            }
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            //var limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            //string sql = BuildSql_Custom_Limit(limit);
            //return dbConnection.Query<TResult>(sql, SqlParams);
            if (dbConnection.ConnInfo.DBType == "MsSql")
            {
                return dbConnection.Query<TResult>(BuildSql_Custom_LimitPageByMsSql(pageNum, pageSize, dbConnection.DBVer), SqlParams);
            }
            else
            {
                return dbConnection.Query<TResult>(BuildSql_Custom_LimitPageByMySql(pageNum, pageSize), SqlParams);
            }
        }

        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            //var limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            //string sql = BuildSql_Custom_Limit(limit);
            //return dbConnection.QueryAsync<TResult>(sql, SqlParams);
            if (dbConnection.ConnInfo.DBType == "MsSql")
            {
                return dbConnection.QueryAsync<TResult>(BuildSql_Custom_LimitPageByMsSql(pageNum, pageSize, dbConnection.DBVer), SqlParams);
            }
            else
            {
                return dbConnection.QueryAsync<TResult>(BuildSql_Custom_LimitPageByMySql(pageNum, pageSize), SqlParams);
            }
        }

        public PageResult<T> QueryPages(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<T> pageResult = new PageResult<T>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.ArrayData = QueryWithPage(pageNum, pageSize);
            pageResult.TotalCount = Count();
            return pageResult;

        }
        public async Task<PageResult<T>> QueryPagesAsync(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<T> pageResult = new PageResult<T>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.ArrayData = await QueryWithPageAsync(pageNum, pageSize);
            pageResult.TotalCount = Count();
            return pageResult;

        }

        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            pageResult.TotalCount = Count();
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            pageResult.TotalCount = Count();
            return pageResult;
        }

        public int Count()
        {
            string sql = "";
            if (string.IsNullOrEmpty(GroupByStr))
            {
                sql = BuildSql_Custom_Fields_Func("Count(*)");
            }
            else
            {
                sql = $"select Count(*) from ({BuildSql_Custom_Fields_Func("Count(*)")}) as TempTable";
            }
            return dbConnection.ExecuteScalar<int>(sql, SqlParams);
        }

        public Task<int> CountAsync()
        {
            string sql = "";
            if (string.IsNullOrEmpty(GroupByStr))
            {
                sql = BuildSql_Custom_Fields_Func("Count(*)");
            }
            else
            {
                sql = $"select Count(*) from ({BuildSql_Custom_Fields_Func("Count(*)")}) as TempTable";
            }
            return dbConnection.ExecuteScalarAsync<int>(sql, SqlParams);
        }

        public TResult Max<TResult>(Expression<Func<T, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }

        public TResult Min<TResult>(Expression<Func<T, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }

        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression)
        {
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:dbConnection.Tag);
            string sql = BuildSql_Custom_Fields_Func("Sum(" + ps.SqlStr + ")");
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: dbConnection.Tag);
            string sql = BuildSql_Custom_Fields_Func("Sum(" + ps.SqlStr + ")");
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Avg<TResult>(Expression<Func<T, TResult>> expression)
        {
            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: dbConnection.Tag);
            string sql = BuildSql_Custom_Fields_Func("Avg(" + ps.SqlStr + ")");
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression)
        {
            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: dbConnection.Tag);
            string sql = BuildSql_Custom_Fields_Func("Avg(" + ps.SqlStr + ")");
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }

        public int Remove(Expression<Func<T, bool>> expression)
        {
            ExpressionResolver.ResoveWhereExpression(this, expression);
            var nea = Tools.GetNEA(typeof(T),dbConnection.Tag);

            string sql = $"Delete from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} where {WhereStr} ";
            return dbConnection.Execute(sql, SqlParams);
        }

        public Task<int> RemoveAsync(Expression<Func<T, bool>> expression)
        {
            ExpressionResolver.ResoveWhereExpression(this, expression);
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);

            string sql = $"Delete from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} where {WhereStr} ";
           
            return dbConnection.ExecuteAsync(sql, SqlParams);
        }

        public int RemoveByPrimaryKeys(string[] primaryKeys)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            var pks = nea.PrimaryKey.Split(',').ToList();
            if (pks.Count != primaryKeys.Length)
            {
                SqlLogUtil.SendLog(LogType.Error, "DeleteByPrimaryKeys:主键参数数量不一致");
                return -1;
            }
            else
            {
                List<string> wheres = new List<string>();
                for (int i = 0; i < pks.Count; i++)
                {
                    var key = Tools.RandomCode(6, 0, true);
                    wheres.Add($"{nea._Lr}{pks[i]}{nea._Rr}=@{key}");
                    SqlParams.Add(key, primaryKeys[i]);
                }
                string sql = $"Delete from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} where {string.Join(" and ", wheres)};";
             
                return dbConnection.Execute(sql, SqlParams);
            }

        }

        public Task<int> RemoveByPrimaryKeysAsync(string[] primaryKeys)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            var pks = nea.PrimaryKey.Split(',').ToList();
            if (pks.Count != primaryKeys.Length)
            {
                SqlLogUtil.SendLog(LogType.Error, "DeleteByPrimaryKeys:主键参数数量不一致");
                return Task.FromResult(-1);
            }
            else
            {
                List<string> wheres = new List<string>();
                for (int i = 0; i < pks.Count; i++)
                {
                    var key = Tools.RandomCode(6, 0, true);
                    wheres.Add($"{nea._Lr}{pks[i]}{nea._Rr}=@{key}");
                    SqlParams.Add(key, primaryKeys[i]);
                }
                string sql = $"Delete from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} where {string.Join(" and ", wheres)};";
               
                return dbConnection.ExecuteAsync(sql, SqlParams);
            }

        }



       

        public int Update<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(expressionWhere.Body, tag: dbConnection.Tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            string sql = $"update {nea.TableSuffixName(TableSuffix)} set {setStr} where {ps_where.SqlStr};";
          
            return dbConnection.Execute(sql, ps.Params);
        }

        public Task<int> UpdateAsync<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew, dbConnection.Tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(expressionWhere.Body, tag: dbConnection.Tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            string sql = $"update {nea.TableSuffixName(TableSuffix)} set {setStr} where {ps_where.SqlStr};";
           
            return dbConnection.ExecuteAsync(sql, ps.Params);
        }


        #region 联合查询


        IQingQuery.IQingQuery<T, T2> IQingQuery<T>.LeftJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn)
        {
            var nea = Tools.GetNEA(typeof(T2), dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            string joinStr = $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2>(this, joinStr, SqlParams);

        }

        public IQingQuery.IQingQuery<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn) where T2 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T2), dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            string joinStr = $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2>(this, joinStr, SqlParams);
        }

        public IQingQuery.IQingQuery<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn) where T2 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T2), dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            string joinStr = $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2>(this, joinStr, SqlParams);
        }



        public QingUnionQuery<TResult> UnionSelect<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere)
        {
            ExpressionResolver.ResoveWhereExpression(this, expressionWhere, dbConnection.Tag);
            var result = ExpressionResolver.ResoveExpression(expressionNew.Body, tag: dbConnection.Tag);
            Fields = result.SqlStr;
            SqlParams.AddDynamicParams(result.Params);
            var sql = BuildSql();
            return new QingUnionQuery<TResult>(dbConnection, sql, SqlParams);
        }


        #endregion

        #region BuildSql

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($" {Limit}");
            }
            string sql= sb_sql.ToString();
           
            return sql;
        }

        private string BuildSql_Custom_LimitCount(int count)
        {

            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);// Tools.GetNEA(typeof(T));
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";
            string top = string.Empty;
            if (nea._dbType == "MsSql")
            {
                top = "top " + count;
            }
            StringBuilder sb_sql = new StringBuilder($"select {top} {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (nea._dbType != "MsSql")
            {
                sb_sql.Append($" limit {count}");
            }
            string sql = sb_sql.ToString();
          
            return sql;
        }

        private string BuildSql_Custom_LimitPageByMySql(int pageNum, int pageSize)
        {

            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);// Tools.GetNEA(typeof(T));
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }

            sb_sql.Append($"limit {pageSize * (pageNum - 1)},{pageSize}");
            string sql = sb_sql.ToString();
           
            return sql;
        }

        private string BuildSql_Custom_LimitPageByMsSql(int pageNum, int pageSize, int dbVer)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";

            string where = string.Empty;
            if (!string.IsNullOrEmpty(WhereStr))
            {
                where = " where " + WhereStr;
            }

            string group = string.Empty;
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                group = " Group By  " + GroupByStr.TrimStart(',');
            }

            var orderBy = string.Empty;
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                orderBy = $" Order By {OrderByStr.TrimStart(',')}";
            }
            else
            {
                if (string.IsNullOrEmpty(nea.PrimaryKey))
                {
                    orderBy = " Order By  GetDate()";
                }
                else
                {
                    orderBy = $" Order By {string.Join(",", nea.PrimaryKey.Split(',').Select(x => $"{nea._Lr}{x}{nea._Rr}").ToArray())}";
                }
            }
            string sql = null;
            if (dbVer <= 10)
            {
                //sqlserver2012以下版本(不包含) 使用RowNumber分页查询
                //WITH t AS ( SELECT a.[CustomerID], a.[CustomerName], a.[CustomerPhoneNum], ROW_NUMBER() OVER(  ORDER BY a.[CustomerPhoneNum]) AS __rownum__ FROM[Customers] a ) SELECT t.* FROM t where __rownum__ between 5 and 8;
                //WITH t AS ( SELECT [dbo].[customers].[CustomerName],[dbo].[customers].[CustomerPhoneNum] as '手机号', ROW_NUMBER() OVER(   Order By [CustomerID]) AS __rownum__ FROM [Test].[dbo].[customers]    Order By [CustomerID] ) SELECT t.* FROM t where __rownum__ between 1 and 5”

                sql= $"WITH t AS ( SELECT {fieldsStr.TrimStart(',')}, ROW_NUMBER() OVER(  {orderBy}) AS __rownum__ FROM {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} {where} {group}  ) SELECT t.* FROM t where __rownum__ between {(pageNum-1)*pageSize+1} and {pageNum*pageSize}";
            }
            else
            {
                //sqlserver2012及以上(包含) 使用offset fetch next分页查询
                //SELECT a.[CustomerID], a.[CustomerName], a.[CustomerPhoneNum] FROM[Customers] a ORDER BY a.[CustomerPhoneNum] OFFSET 4 ROW FETCH NEXT 4 ROW ONLY;
                sql=  $"SELECT {fieldsStr.TrimStart(',')} FROM {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)} {where} {group} {orderBy} OFFSET {(pageNum - 1) * pageSize } ROW FETCH NEXT {pageSize} ROW ONLY";
            }

           
            return sql;
        }

        private string BuildSql_Custom_Fields(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($" {Limit}");
            }
            string sql = sb_sql.ToString();
          
            return sql;
        }
        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            string sql = sb_sql.ToString();
          
            return sql;
        }

        private string BuildSql_Custom_WhereStr(string whereStr)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(whereStr))
            {
                sb_sql.Append($" where {whereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($" {Limit}");
            }
            string sql = sb_sql.ToString();
          
            return sql;
        }

        private string BuildSql_Custom_OrderByStr(string orderByStr)
        {
            var nea = Tools.GetNEA(typeof(T), dbConnection.Tag);
            if (string.IsNullOrEmpty(Fields) && SelectAlias)
            {
                var fas = Tools.GetFieldsAlias(typeof(T));
                StringBuilder sb = new StringBuilder();
                foreach (var fa in fas)
                {
                    if (string.IsNullOrEmpty(fa.Value))
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr},");
                    }
                    else
                    {
                        sb.Append($"{nea._Lr}{fa.Key}{nea._Rr} as '{fa.Value}',");
                    }

                }

                Fields = sb.ToString().TrimEnd(',');
            }
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableSuffixName(TableSuffix)}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(orderByStr))
            {
                sb_sql.Append($" Order By {orderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($" {Limit}");
            }
            string sql = sb_sql.ToString();
         
            return sql;
        }

        public ParamSql BuildQuery()
        {
            ParamSql paraSql = new ParamSql();
            paraSql.SqlStr = BuildSql();
            paraSql.Params.AddDynamicParams(SqlParams);
            return paraSql;
        }

        public ParamSql BuildFirstQuery()
        {
            ParamSql paraSql = new ParamSql();
            paraSql.SqlStr = BuildSql_Custom_LimitCount(1);
            paraSql.Params.AddDynamicParams(SqlParams);
            return paraSql;
        }

        #endregion
    }
}
