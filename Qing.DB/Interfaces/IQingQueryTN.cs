﻿using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.IQingQuery
{
    #region T2
    public interface IQingQuery<T, T2>
    where T : BaseEntity
    where T2 : BaseEntity
    {
        IQingQuery<T, T2, T3> LeftJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity;
        IQingQuery<T, T2, T3> RightJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity;
        IQingQuery<T, T2, T3> InnerJoin<T3>(Expression<Func<T, T2, T3, bool>> expressionJoinOn) where T3 : BaseEntity;
        IQingQuery<T, T2> Select<TResult>(Expression<Func<T, T2, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2> Where(Expression<Func<T, T2, bool>> expression);
        IQingQuery<T, T2> OrderBy<TResult>(Expression<Func<T, T2, TResult>> expression, bool Desc = false);
        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);
        IQingQuery<T, T2> GroupBy<TResult>(Expression<Func<T, T2, TResult>> expression);
    }
    #endregion

    #region T3
    public interface IQingQuery<T, T2, T3>
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
    {

        IQingQuery<T, T2, T3, T4> LeftJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity;
        IQingQuery<T, T2, T3, T4> RightJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity;
        IQingQuery<T, T2, T3, T4> InnerJoin<T4>(Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn) where T4 : BaseEntity;
        IQingQuery<T, T2, T3> Select<TResult>(Expression<Func<T, T2, T3, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3> Where(Expression<Func<T, T2, T3, bool>> expression);
        IQingQuery<T, T2, T3> OrderBy<TResult>(Expression<Func<T, T2, T3, TResult>> expression, bool Desc = false);
       

        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2,T3, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);
        IQingQuery<T, T2,T3> GroupBy<TResult>(Expression<Func<T, T2,T3,TResult>> expression);
    }
    #endregion

    #region T4
    public interface IQingQuery<T, T2, T3, T4>
     where T : BaseEntity
     where T2 : BaseEntity
     where T3 : BaseEntity
     where T4 : BaseEntity
    {
        IQingQuery<T, T2, T3, T4, T5> LeftJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5> RightJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5> InnerJoin<T5>(Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn) where T5 : BaseEntity;
        IQingQuery<T, T2, T3, T4> Select<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4> Where(Expression<Func<T, T2, T3, T4, bool>> expression);
        IQingQuery<T, T2, T3, T4> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression, bool Desc = false);


        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3,T4, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);
        IQingQuery<T, T2, T3,T4> GroupBy<TResult>(Expression<Func<T, T2, T3,T4,TResult>> expression);
    }
    #endregion

    #region T5
    public interface IQingQuery<T, T2, T3, T4, T5>
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
    where T4 : BaseEntity
    where T5 : BaseEntity
    {

        IQingQuery<T, T2, T3, T4, T5> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5> Where(Expression<Func<T, T2, T3, T4, T5, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool Desc = false);
        IQingQuery<T, T2, T3, T4, T5, T6> LeftJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6> RightJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6> InnerJoin<T6>(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn) where T6 : BaseEntity;

        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);

        IQingQuery<T, T2, T3, T4,T5> GroupBy<TResult>(Expression<Func<T, T2, T3, T4,T5, TResult>> expression);
    }
    #endregion

    #region T6
    public interface IQingQuery<T, T2, T3, T4, T5, T6>
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
    where T4 : BaseEntity
    where T5 : BaseEntity
    where T6 : BaseEntity
    {
        IQingQuery<T, T2, T3, T4, T5, T6> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5, T6> Where(Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5, T6> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool Desc = false);
        IQingQuery<T, T2, T3, T4, T5, T6,T7> LeftJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6,T7> RightJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6,T7> InnerJoin<T7>(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn) where T7 : BaseEntity;


        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);


        IQingQuery<T, T2, T3, T4, T5,T6> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5,T6, TResult>> expression);
    }
    #endregion

    #region T7
    public interface IQingQuery<T, T2, T3, T4, T5, T6, T7>
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
    where T4 : BaseEntity
    where T5 : BaseEntity
    where T6 : BaseEntity
    where T7 : BaseEntity
    {
        IQingQuery<T, T2, T3, T4, T5, T6, T7> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5, T6, T7> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool Desc = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7,T8> LeftJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7,T8> RightJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7,T8> InnerJoin<T8>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn) where T8 : BaseEntity;

        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);

        IQingQuery<T, T2, T3, T4, T5, T6,T7> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6,T7, TResult>> expression);
    }
    #endregion

    #region T8
    public interface IQingQuery<T, T2, T3, T4, T5, T6, T7, T8>
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
    where T4 : BaseEntity
    where T5 : BaseEntity
    where T6 : BaseEntity
    where T7 : BaseEntity
    where T8 : BaseEntity
    {
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool Desc = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8,T9> LeftJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8,T9> RightJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8,T9> InnerJoin<T9>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn) where T9 : BaseEntity;

        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);

        IQingQuery<T, T2, T3, T4, T5, T6, T7,T8> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, TResult>> expression);
    }
    #endregion

    #region T9
    public interface IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9>
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
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool Desc = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> LeftJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expressionJoinOn) where T10 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> RightJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expressionJoinOn) where T10 : BaseEntity;
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> InnerJoin<T10>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, bool>> expressionJoinOn) where T10 : BaseEntity;

        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);

        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8,T9> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7,T8, T9, TResult>> expression);
    }
    #endregion

    #region T10
    public interface IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>
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
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Select<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool selectAlia = false);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> Where(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> OrderBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool Desc = false);
        IEnumerable<TResult> QueryAll<TResult>();
        Task<IEnumerable<TResult>> QueryAllAsync<TResult>();
        TResult QueryFirst<TResult>();
        Task<TResult> QueryFirstAsync<TResult>();
        int Count();
        Task<int> CountAsync();
        IEnumerable<TResult> QueryWithPage<TResult>(int pageNum, int pageSize);
        Task<IEnumerable<TResult>> QueryWithPageAsync<TResult>(int pageNum, int pageSize);
        TResult Max<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        TResult Min<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        Task<TResult> MinAsync<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        PageResult<TResult> QueryPages<TResult>(int pageNum, int pageSize);
        Task<PageResult<TResult>> QueryPagesAsync<TResult>(int pageNum, int pageSize);
        IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9,T10> GroupBy<TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9,T10, TResult>> expression);

    }
    #endregion

}
