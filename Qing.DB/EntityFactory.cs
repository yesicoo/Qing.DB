using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Qing.DB.Model;

namespace Qing.DB
{
    public static class EntityFactory<T> where T : BaseEntity
    {
        /// <summary>
        /// 条件更新
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expressionNew"></param>
        /// <param name="whereExpression"></param>
        /// <param name="dbid"></param>
        /// <returns></returns>
        public static int Update<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> whereExpression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew,tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            dbid = Tools.GetDBID(dbid);
            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";
          
            return DBFactory.Instance.Execute(dbid, sql, ps.Params, tag);

        }

        public static Task<int> UpdateAsync<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> whereExpression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew,tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            dbid = Tools.GetDBID(dbid);
            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";
            
            return DBFactory.Instance.ExecuteAsync(dbid, sql, ps.Params,tag);

        }

        /// <summary>
        /// 条件更新
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expressionNew"></param>
        /// <param name="whereExpression"></param>
        /// <param name="dbid"></param>
        /// <returns></returns>
        public static int Update<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> whereExpression, IDbTransaction transaction, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew,tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_where.Params);
            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";
           
            return transaction.Connection.Execute(sql, ps.Params,transaction);

        }

        public static Task<int> UpdateAsync<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> whereExpression, IDbTransaction transaction, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T), tag);

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew,tag);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_where.Params);
            string sql = $"update {nea.TableSuffixName(tableSuffix)} set {setStr} where {ps_where.SqlStr};";
           
            return transaction.Connection.ExecuteAsync(sql, ps.Params, transaction);

        }

        public static int Insert(T t, string dbid = null, string tableSuffix = null, string tag = null)
        {
           return t.Insert(dbid,tableSuffix,tag:tag);
        }

        public static Task<int> InsertAsync(T t, string dbid = null, string tableSuffix = null, string tag = null)
        {
            return Task.FromResult(t.Insert(dbid, tableSuffix,tag:tag));
        }

        public static int Insert(IEnumerable<T> ts, string dbid = null, string tableSuffix = null, string tag = null)
        {
           return ts.ToList().BulkInsert(dbid,tableSuffix,tag:tag);
        }
        public static Task<int> InsertAsync(IEnumerable<T> ts, string dbid = null, string tableSuffix = null, string tag = null)
        {
            return ts.ToList().BulkInsertAsync(dbid,tableSuffix,tag:tag);
        }

        public static int Delete(Expression<Func<T, bool>> whereExpression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            string sql = $"delete from  {nea.TableSuffixName(tableSuffix)}  where {ps_where.SqlStr};";
           
            return DBFactory.Instance.Execute(dbid, sql, ps_where.Params,tag);
        }
        public static Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            string sql = $"delete from  {nea.TableSuffixName(tableSuffix)}  where {ps_where.SqlStr};";
            return DBFactory.Instance.ExecuteAsync(dbid, sql, ps_where.Params, tag);
        }

        public static int Delete(Expression<Func<T, bool>> whereExpression, IDbTransaction transaction, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T), tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            string sql = $"delete from  {nea.TableSuffixName(tableSuffix)}  where {ps_where.SqlStr};";
            return transaction.Connection.Execute(sql, ps_where.Params,transaction);
        }
        public static Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpression, IDbTransaction transaction, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            string sql = $"delete from  {nea.TableSuffixName(tableSuffix)}  where {ps_where.SqlStr};";
            return transaction.Connection.ExecuteAsync(sql, ps_where.Params, transaction);
        }


        #region Select

        /// <summary>
        /// 查询单个
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static T QueryFirst(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)}  ");
            if (expression == null)
            {
                return DBFactory.Instance.QueryFirstOrDefault<T>(dbid, sb.ToString(), tag);
            }
            else
            {
                var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
                sb.Append($" where {ps.SqlStr} ");
                return DBFactory.Instance.QueryFirstOrDefault<T>(dbid, sb.ToString(), ps.Params,tag);
            }
        }

        public static Task<T> QueryFirstAsync(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)}  ");
            if (expression == null)
            {
                return DBFactory.Instance.QueryFirstOrDefaultAsync<T>(dbid, sb.ToString(),tag);
            }
            else
            {
                var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: tag);
                sb.Append($" where {ps.SqlStr} ");
                return DBFactory.Instance.QueryFirstOrDefaultAsync<T>(dbid, sb.ToString(), ps.Params,tag);
            }
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>

        public static IEnumerable<T> Query( string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            string sql = $"select * from {nea.TableSuffixName(tableSuffix)};";
            return DBFactory.Instance.Query<T>(dbid, sql,tag);
        }

        public static Task<IEnumerable<T>> QueryAsync(string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            string sql = $"select * from {nea.TableSuffixName(tableSuffix)};";
            return DBFactory.Instance.QueryAsync<T>(dbid, sql,tag);
        }


        public static IEnumerable<T> Query( int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);
            string sql =$"select * from {nea.TableSuffixName(tableSuffix)} limit {pageSize * (pageNum - 1)},{pageSize}";
            return DBFactory.Instance.Query<T>(dbid, sql,tag);
        }

        public static Task<IEnumerable<T>> QueryAsync(int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);
            string sql = $"select * from {nea.TableSuffixName(tableSuffix)} limit {pageSize * (pageNum - 1)},{pageSize}";
            return DBFactory.Instance.QueryAsync<T>(dbid, sql,tag);
        }

        public static IEnumerable<T> Query(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps.SqlStr);
            return DBFactory.Instance.Query<T>(dbid, sb.ToString(), ps.Params,tag);
        }

        public static Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps.SqlStr);
            return DBFactory.Instance.QueryAsync<T>(dbid, sb.ToString(), ps.Params,tag);
        }

        public static int QueryCount(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select Count(*) from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps.SqlStr);
            return DBFactory.Instance.QueryFirstOrDefault<int>(dbid, sb.ToString(), ps.Params,tag);
        }

        public static Task<int> QueryCountAsync(Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T), tag);
            StringBuilder sb = new StringBuilder($"select Count(*) from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps.SqlStr);
            return DBFactory.Instance.QueryFirstOrDefaultAsync<int>(dbid, sb.ToString(), ps.Params,tag);
        }

        public static IEnumerable<T> Query(Expression<Func<T, bool>> expression, int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: tag);
            sb.Append(ps.SqlStr);
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.Query<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> expression, int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps.SqlStr);
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.QueryAsync<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static IEnumerable<T> Query<TOrder>(Expression<Func<T, bool>> expression, int PageNum, int PageSize, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);

            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: tag);
            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.Query<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static Task<IEnumerable<T>> QueryAsync<TOrder>(Expression<Func<T, bool>> expression, int PageNum, int PageSize, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);

            var ps = ExpressionResolver.ResoveExpression(expression.Body, tag: tag);
            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.QueryAsync<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static IEnumerable<T> Query<TOrder>(Expression<Func<T, bool>> expression, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_order.Params);
            sb.Append(ps.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            return DBFactory.Instance.Query<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static Task<IEnumerable<T>> QueryAsync<TOrder>(Expression<Func<T, bool>> expression, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select * from {nea.TableSuffixName(tableSuffix)} where ");
            var ps = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_order.Params);
            sb.Append(ps.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            return DBFactory.Instance.QueryAsync<T>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static IEnumerable<TSelect> Query<TSelect>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            var ps2 = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps2.SqlStr);
            ps.Params.AddDynamicParams(ps2.Params);
            return DBFactory.Instance.Query<TSelect>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static Task<IEnumerable<TSelect>> QueryAsync<TSelect>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> expression, string dbid = null, string tableSuffix = null, string tag = null)
        {
            var nea = Tools.GetNEA(typeof(T), tag);
            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body,tag:tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            var ps2 = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps2.SqlStr);
            ps.Params.AddDynamicParams(ps2.Params);
            return DBFactory.Instance.QueryAsync<TSelect>(dbid, sb.ToString(), ps.Params,tag);
        }
        public static IEnumerable<TSelect> Query<TSelect>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> expression, int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T), tag);
            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            var ps2 = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps2.SqlStr);
            ps.Params.AddDynamicParams(ps2.Params);
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.Query<TSelect>(dbid, sb.ToString(), ps.Params, tag);
        }

        public static Task<IEnumerable<TSelect>> QueryAsync<TSelect>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> expression, int PageNum, int PageSize, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;
            var nea = Tools.GetNEA(typeof(T),tag);
            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            var ps2 = ExpressionResolver.ResoveExpression(expression.Body,tag:tag);
            sb.Append(ps2.SqlStr);
            ps.Params.AddDynamicParams(ps2.Params);
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.QueryAsync<TSelect>(dbid, sb.ToString(), ps.Params, tag);
        }

        public static IEnumerable<TSelect> Query<TSelect, TOrder>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> whereExpression, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {

            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            var nea = Tools.GetNEA(typeof(T), tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps_where.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");

            return DBFactory.Instance.Query<TSelect>(dbid, sb.ToString(), ps.Params, tag);
        }

        public static Task<IEnumerable<TSelect>> QueryAsync<TSelect, TOrder>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> whereExpression, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {

            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            var nea = Tools.GetNEA(typeof(T), tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps_where.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");

            return DBFactory.Instance.QueryAsync<TSelect>(dbid, sb.ToString(), ps.Params, tag);
        }

        public static IEnumerable<TSelect> Query<TSelect, TOrder>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> whereExpression, int PageNum, int PageSize, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;

            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body, tag: tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body, tag: tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps_where.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.Query<TSelect>(dbid, sb.ToString(), ps.Params,tag);

        }

        public static Task<IEnumerable<TSelect>> QueryAsync<TSelect, TOrder>(Expression<Func<T, TSelect>> selectExpression, Expression<Func<T, bool>> whereExpression, int PageNum, int PageSize, Expression<Func<T, TOrder>> orderyExpression, bool orderByDesc = false, string dbid = null, string tableSuffix = null, string tag = null)
        {
            int pageNum = PageNum > 0 ? PageNum : 1;
            int pageSize = PageSize > 0 ? PageSize : 1;

            var ps = ExpressionResolver.ResoveExpression(selectExpression.Body,tag:tag);
            var ps_where = ExpressionResolver.ResoveExpression(whereExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_where.Params);

            var ps_order = ExpressionResolver.ResoveExpression(orderyExpression.Body,tag:tag);
            ps.Params.AddDynamicParams(ps_order.Params);

            var nea = Tools.GetNEA(typeof(T),tag);
            StringBuilder sb = new StringBuilder($"select {ps.SqlStr} from {nea.TableSuffixName(tableSuffix)} where ");
            sb.Append(ps_where.SqlStr);
            sb.Append($" Order By {ps_order.SqlStr} {(orderByDesc ? "Desc" : "")}");
            sb.Append($" limit {pageSize * (pageNum - 1)},{pageSize}");
            return DBFactory.Instance.QueryAsync<TSelect>(dbid, sb.ToString(), ps.Params,tag);

        }
        #endregion
    }
}
