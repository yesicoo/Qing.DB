using Dapper;
using Qing.DB.IQingQuery;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.QingQuery
{
    #region T2
    internal class QingQuery<T, T2> : IQingQuery<T, T2> where T : BaseEntity where T2 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { get; set; }
        internal string JoinStr { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();



        public QingQuery(QingQuery<T> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }


        public IQingQuery<T, T2> OrderBy<TResult>(Expression<Func<T, T2, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc,tag:dbConnection.Tag);
        }
        public IQingQuery<T, T2> GroupBy<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2> Select<TResult>(Expression<Func<T, T2, TResult>> expression, bool selectAlia = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlia,dbConnection.Tag);
        }

        public IQingQuery<T, T2> Where(Expression<Func<T, T2, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }
        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData =await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag:dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T3), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T3), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T3), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";
            return new QingQuery<T, T2, T3>(this, JoinStr, SqlParams);
        }


    }
    #endregion

    #region T3

    internal class QingQuery<T, T2, T3> : IQingQuery<T, T2, T3>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
    {

        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }

        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();



        public QingQuery(QingQuery<T, T2> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }


        public IQingQuery<T, T2, T3> OrderBy<TResult>(Expression<Func<T, T2, T3, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression,bool selectAlias=false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression,selectAlias,tag: dbConnection.Tag);

        }

        public IQingQuery<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }
        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T4), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T4), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T4), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3> GroupBy<TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T4

    internal class QingQuery<T, T2, T3, T4> : IQingQuery<T, T2, T3, T4>
           where T : BaseEntity
           where T2 : BaseEntity
           where T3 : BaseEntity
            where T4 : BaseEntity
    {


        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();

        public QingQuery(QingQuery<T, T2, T3> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }
       
        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }
        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }


        public IQingQuery<T, T2, T3, T4> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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

        public IQingQuery<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T5), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T5), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T5), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T5

    internal class QingQuery<T, T2, T3, T4, T5> : IQingQuery<T, T2, T3, T4, T5>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();



        public QingQuery(QingQuery<T, T2, T3, T4> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }
       
        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }
        public IQingQuery<T, T2, T3, T4, T5> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4,T5, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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
        public IQingQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T6), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T6), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6>(this, JoinStr, jps.Params);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T6), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6>(this, JoinStr, jps.Params);
        }

        public IQingQuery<T, T2, T3, T4, T5> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T6

    internal class QingQuery<T, T2, T3, T4, T5, T6> : IQingQuery<T, T2, T3, T4, T5, T6>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
           where T6 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }
        internal DynamicParameters SqlParams = new DynamicParameters();

        public QingQuery(QingQuery<T, T2, T3, T4, T5> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;

        }
       


        public IQingQuery<T, T2, T3, T4, T5, T6> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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
        public IQingQuery<T, T2, T3, T4, T5, T6, T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T7), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T7), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T7), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);

        }

        public IQingQuery<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5, T6> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T7

    internal class QingQuery<T, T2, T3, T4, T5, T6, T7> : IQingQuery<T, T2, T3, T4, T5, T6, T7>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
           where T6 : BaseEntity
        where T7 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }

        internal DynamicParameters SqlParams = new DynamicParameters();


        public QingQuery(QingQuery<T, T2, T3, T4, T5, T6> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }
       
        public IQingQuery<T, T2, T3, T4, T5, T6, T7> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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

        public IQingQuery<T, T2, T3, T4, T5, T6, T7> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T8), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T8), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T8), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8>(this, JoinStr, SqlParams);
        }

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T8

    internal class QingQuery<T, T2, T3, T4, T5, T6, T7, T8> : IQingQuery<T, T2, T3, T4, T5, T6, T7, T8>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
           where T6 : BaseEntity
        where T7 : BaseEntity
        where T8 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }

        internal DynamicParameters SqlParams = new DynamicParameters();

        public QingQuery(QingQuery<T, T2, T3, T4, T5, T6, T7> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData =new List<TResult>();
            }
            else {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
          
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias,tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T9), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T9), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T9), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>(this, JoinStr, SqlParams);
        }

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T9

    internal class QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> : IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
           where T6 : BaseEntity
        where T7 : BaseEntity
        where T8 : BaseEntity
        where T9 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }

        internal DynamicParameters SqlParams = new DynamicParameters();


        public QingQuery(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;
            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }
        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8,T9, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }


        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expressionJoinOn) where T10 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T10), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Right Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, JoinStr, SqlParams);
        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expressionJoinOn) where T10 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T10), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Inner Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, JoinStr, SqlParams);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expressionJoinOn) where T10 : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T10), tag: dbConnection.Tag);
            var jps = ExpressionResolver.ResoveJoinExpression(this, expressionJoinOn, tag: dbConnection.Tag);
            SqlParams.AddDynamicParams(jps.Params);
            JoinStr += $" Left Join {dbConnection.SqlConnection.Database}.{nea.TableName} on ({ jps.SqlStr})";

            return new QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this, JoinStr, SqlParams);
        }

        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

    #region T10

    internal class QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
           where T6 : BaseEntity
        where T7 : BaseEntity
        where T8 : BaseEntity
        where T9 : BaseEntity
        where T10 : BaseEntity
    {
        internal DBConnectionItem dbConnection { set; get; }
        internal string WhereStr { get; set; } = string.Empty;
        internal string Fields { get; set; }
        internal string Limit { get; set; } = "";
        internal string OrderByStr { get; set; }
        internal string GroupByStr { set; get; }
        internal string JoinStr { get; set; }

        internal DynamicParameters SqlParams = new DynamicParameters();

        public QingQuery(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> edbq, string joinStr, DynamicParameters sqlParams)
        {
            this.dbConnection = edbq.dbConnection;

            this.Fields = edbq.Fields;
            this.Limit = edbq.Limit;
            this.OrderByStr = edbq.OrderByStr;
            this.JoinStr += joinStr;
            this.SqlParams = sqlParams;
        }

        public TResult QueryFirst<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirst<TResult>(sql, SqlParams);
        }
        public Task<TResult> QueryFirstAsync<TResult>()
        {
            Limit = $"limit 1";
            string sql = BuildSql();
            return dbConnection.QueryFirstAsync<TResult>(sql, SqlParams);
        }
        public IEnumerable<TResult> QueryWithPage<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAll<TResult>();
        }
        public Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            Limit = $"limit {pageSize * (pageNum - 1)},{pageSize}";
            return QueryAllAsync<TResult>();
        }
        public PageResult<TResult> QueryPages<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData =  QueryWithPage<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync<TResult>(int PageNum, int PageSize)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = pageNum;
            pageResult.CurrentPageSize = pageSize;
            pageResult.TotalCount = Count();
            if (pageResult.TotalCount == 0)
            {
                pageResult.ArrayData = new List<TResult>();
            }
            else
            {
                pageResult.ArrayData = await QueryWithPageAsync<TResult>(pageNum, pageSize);
            }
            return pageResult;
        }

        public IEnumerable<TResult> QueryAll<TResult>()
        {
            return dbConnection.Query<TResult>(BuildSql(), SqlParams);
        }

        public Task<IEnumerable<TResult>> QueryAllAsync<TResult>()
        {
            return dbConnection.QueryAsync<TResult>(BuildSql(), SqlParams);
        }
        public TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);
        }
        public TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            ExpressionResolver.ResoveMaxExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalar<TResult>(sql, SqlParams);
        }
        public Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression)
        {
            ExpressionResolver.ResoveMinExpression(this, expression, tag: dbConnection.Tag);
            string sql = BuildSql();
            return dbConnection.ExecuteScalarAsync<TResult>(sql, SqlParams);

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
            string sql = BuildSql_Custom_Fields_Func("Count(*)");

            return dbConnection.ExecuteScalarAsync<int>(sql, SqlParams);

        }
        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool Desc = false)
        {
            return ExpressionResolver.ResoveOrderByExpression(this, expression, Desc, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool selectAlias = false)
        {
            return ExpressionResolver.ResoveSelectExpression(this, expression, selectAlias, tag: dbConnection.Tag);
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression)
        {
            return ExpressionResolver.ResoveWhereExpression(this, expression, tag: dbConnection.Tag);
        }
        private string BuildSql()
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(OrderByStr))
            {
                sb_sql.Append($" Order By {OrderByStr.TrimStart(',')}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }
            if (!string.IsNullOrEmpty(Limit))
            {
                sb_sql.Append($"  {Limit}");
            }
            return sb_sql.ToString();
        }

        private string BuildSql_Custom_Fields_Func(string fields)
        {
            var nea = Tools.GetNEA(typeof(T), tag: dbConnection.Tag);
            var fieldsStr = fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dbConnection.SqlConnection.Database}.{nea.TableName}");
            sb_sql.Append(JoinStr);
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            if (!string.IsNullOrEmpty(GroupByStr))
            {
                sb_sql.Append($" Group By {GroupByStr}");
            }

            return sb_sql.ToString();
        }

        public IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            return ExpressionResolver.ResoveGroupByExpression(this, expression, tag: dbConnection.Tag);
        }
    }

    #endregion

}
