using Qing.DB.IQingQuery;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Qing.DB
{
    public interface IQingQuery<T> where T : BaseEntity
    {
        /// <summary>
        /// 条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQingQuery<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQingQuery<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression);
        IQingQuery<T> OrderBy(string fieldstr, bool desc = false);
        /// <summary>
        /// 排序 倒叙
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQingQuery<T> OrderByDesc<TResult>(Expression<Func<T, TResult>> expression);

        IQingQuery<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression);


        /// <summary>
        /// 选择字段查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQingQuery<T> Select<TResult>(Expression<Func<T, TResult>> expression);

        /// <summary>
        /// 排除字段查询(除了选择的字段其余的都查询返回)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQingQuery<T> SelectExcept<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> QueryAll();
        Task<IEnumerable<T>> QueryAllAsync();
        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        /// <summary>
        /// 条件查询所有
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<T> QueryAll(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> QueryAllAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T> QueryWithPage(int pageNum, int pageSize);
        Task<IEnumerable<T>> QueryWithPageAsync(int pageNum, int pageSize);
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        /// <summary>
        /// 分页查询 返回分页查询结果对象
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        PageResult<T> QueryPages(int pageNum, int pageSize);
        Task<PageResult<T>> QueryPagesAsync(int pageNum, int pageSize);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);
        /// <summary>
        /// 查询第一个
        /// </summary>
        /// <returns></returns>
        T QueryFirst();
        Task<T> QueryFirstAsync();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();

        /// <summary>
        /// 查询前几
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> QueryTop(int top = 1);
        Task<IEnumerable<T>> QueryTopAsync(int top = 1);
        IEnumerable<TResult> QueryTop<TResult>(int top = 1);
        Task<IEnumerable<TResult>> QueryTopAsync<TResult>(int top = 1);
        /// <summary>
        /// 条件查询第一个
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        T QueryFirst(Expression<Func<T, bool>> expression);
        Task<T> QueryFirstAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <returns></returns>
        int Count();
        Task<int> CountAsync();
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Max<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        TResult Min<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> expression);
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression);
        TResult Avg<TResult>(Expression<Func<T, TResult>> expression);
        Task<TResult> AvgAsync<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 联合查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expressionNew"></param>
        /// <param name="expressionWhere"></param>
        /// <returns></returns>
        QingUnionQuery<TResult> UnionSelect<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        int Remove(Expression<Func<T, bool>> expression);
        Task<int> RemoveAsync(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 主键删除
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        int RemoveByPrimaryKeys(string[] primaryKeys);
        Task<int> RemoveByPrimaryKeysAsync(string[] primaryKeys);
        /// <summary>
        /// 条件更新
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expressionNew"></param>
        /// <param name="expressionWhere"></param>
        /// <returns></returns>
        int Update<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere);
        Task<int> UpdateAsync<TResult>(Expression<Func<T, TResult>> expressionNew, Expression<Func<T, bool>> expressionWhere);

        ParamSql BuildQuery();
        ParamSql BuildFirstQuery();

        /// <summary>
        /// 左联
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expressionJoinOn"></param>
        /// <returns></returns>
        IQingQuery<T, T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn) where T2 : BaseEntity;
        /// <summary>
        /// 右联
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expressionJoinOn"></param>
        /// <returns></returns>
        IQingQuery<T, T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn) where T2 : BaseEntity;
        /// <summary>
        /// 内联
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="expressionJoinOn"></param>
        /// <returns></returns>
        IQingQuery<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expressionJoinOn) where T2 : BaseEntity;


        IQingQuery<T> ResolveFilterCondition(SqlFilter fc);

    }
}
