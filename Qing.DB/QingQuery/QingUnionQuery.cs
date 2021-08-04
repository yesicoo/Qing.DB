using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Qing.DB.Model;
using Qing.DB.Model;

namespace Qing.DB
{
    public class QingUnionQuery<TResult>
    {
        private DBConnectionItem dBConnectionItem;
        private string sql;
        private string orderBy = string.Empty;
        private DynamicParameters SqlParams = new DynamicParameters();

        public QingUnionQuery(DBConnectionItem dBConnectionItem, string sql, DynamicParameters sqlParams)
        {
            this.dBConnectionItem = dBConnectionItem;
            this.sql = sql;
            this.SqlParams = sqlParams;
        }

        public QingUnionQuery<TResult> UnionAll<T>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere) where T : BaseEntity, new()
        {

            T t = new T();

            var wherePS = ExpressionResolver.ResoveExpression(expressionWhere.Body);
            SqlParams.AddDynamicParams(wherePS.Params);
            var WhereStr = $"({wherePS.SqlStr})";

            var fidlsPS = ExpressionResolver.ResoveExpression(expressionNew.Body);
            SqlParams.AddDynamicParams(fidlsPS.Params);
            var Fields = fidlsPS.SqlStr;

            var nea = t.GetNEA();
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dBConnectionItem.SqlConnection.Database}.{nea.TableName}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            this.sql = $"({this.sql}) Union All ({sb_sql.ToString()})";

            return this;
        }
        public QingUnionQuery<TResult> Union<T>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere) where T : BaseEntity, new()
        {
            T t = new T();
            var wherePS = ExpressionResolver.ResoveExpression(expressionWhere.Body);
            SqlParams.AddDynamicParams(wherePS.Params);
            var WhereStr = $"({wherePS.SqlStr})";

            var fidlsPS = ExpressionResolver.ResoveExpression(expressionNew.Body);
            SqlParams.AddDynamicParams(fidlsPS.Params);
            var Fields = fidlsPS.SqlStr;

            var nea = t.GetNEA();
            var fieldsStr = Fields ?? "*";
            StringBuilder sb_sql = new StringBuilder($"select {fieldsStr.TrimStart(',')} from {dBConnectionItem.SqlConnection.Database}.{nea.TableName}");
            if (!string.IsNullOrEmpty(WhereStr))
            {
                sb_sql.Append($" where {WhereStr}");
            }
            this.sql = $"({this.sql}) Union  ({sb_sql.ToString()})";

            return this;
        }
        public IEnumerable<TResult> QueryAll()
        {
            string sql = $"select * from ({this.sql}) as UnionView {orderBy}";

            return dBConnectionItem.Query<TResult>(sql, SqlParams);

        }

        public Task<IEnumerable<TResult>> QueryAllAsync()
        {
            string sql = $"select * from ({this.sql}) as UnionView {orderBy}";

            return dBConnectionItem.QueryAsync<TResult>(sql, SqlParams);

        }

        public IEnumerable<TResult> QueryWithPage(int PageNum, int PageSize)
        {
            string sql = $"select * from ({this.sql}) as UnionView {orderBy.TrimStart(',')} limit {(PageNum - 1) * PageSize},{PageSize}";

            return dBConnectionItem.Query<TResult>(sql, SqlParams);

        }

        public Task<IEnumerable<TResult>> QueryWithPageAsync(int PageNum, int PageSize)
        {
            string sql = $"select * from ({this.sql}) as UnionView {orderBy.TrimStart(',')} limit {(PageNum - 1) * PageSize},{PageSize}";

            return dBConnectionItem.QueryAsync<TResult>(sql, SqlParams);

        }

        public PageResult<TResult> QueryPages(int PageNum, int PageSize)
        {
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = PageNum;
            pageResult.CurrentPageSize = PageSize;
            string sql = $"select * from ({this.sql}) as UnionView {orderBy.TrimStart(',')} limit {(PageNum - 1) * PageSize},{PageSize}";
            pageResult.ArrayData = dBConnectionItem.Query<TResult>(sql, SqlParams);
            pageResult.TotalCount = dBConnectionItem.QueryFirst<int>($"select count(*) from ({this.sql}) as UnionView");
            return pageResult;
        }

        public async Task<PageResult<TResult>> QueryPagesAsync(int PageNum, int PageSize)
        {
            PageResult<TResult> pageResult = new PageResult<TResult>();
            pageResult.CurrentPageNum = PageNum;
            pageResult.CurrentPageSize = PageSize;
            string sql = $"select * from ({this.sql}) as UnionView {orderBy.TrimStart(',')} limit {(PageNum - 1) * PageSize},{PageSize}";
            pageResult.ArrayData = await dBConnectionItem.QueryAsync<TResult>(sql, SqlParams);
            pageResult.TotalCount = await dBConnectionItem.QueryFirstAsync<int>($"select count(*) from ({this.sql}) as UnionView");
            return pageResult;
        }

        public QingUnionQuery<TResult> OrderBy(Expression<Func<TResult, object>> expression, bool desc = false)
        {
            var ps = ExpressionResolver.ResoveExpression(expression.Body);
            orderBy += ",Order By" + ps.SqlStr + (desc ? " Desc" : "");
            this.SqlParams.AddDynamicParams(ps.Params);
            return this;
        }

        public int Count()
        {
            string sql = $"select count(*) from ({this.sql}) as UnionView ";

            return dBConnectionItem.QueryFirst<int>(sql, SqlParams);
        }

        public Task<int> CountAsync()
        {
            string sql = $"select count(*) from ({this.sql}) as UnionView ";

            return dBConnectionItem.QueryFirstAsync<int>(sql, SqlParams);
        }
    }
}
