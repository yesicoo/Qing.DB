using Qing.DB.QingQuery;
using Qing.DB.IQingQuery;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Qing.DB
{
    internal static class ExpressionResolver
    {

        #region Where
        internal static QingQuery<T> ResoveWhereExpression<T>(QingQuery<T> nt, Expression<Func<T, bool>> expression,string tag=null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body,tag:tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2> ResoveWhereExpression<T, T2>(QingQuery<T, T2> nt, Expression<Func<T, T2, bool>> expression, string tag = null)
              where T : BaseEntity
           where T2 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2, T3> ResoveWhereExpression<T, T2, T3>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, bool>> expression, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4> ResoveWhereExpression<T, T2, T3, T4>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, bool>> expression, string tag = null)
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
          where T4 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5> ResoveWhereExpression<T, T2, T3, T4, T5>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, bool>> expression, string tag = null)
         where T : BaseEntity
         where T2 : BaseEntity
         where T3 : BaseEntity
         where T4 : BaseEntity
         where T5 : BaseEntity

        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6> ResoveWhereExpression<T, T2, T3, T4, T5, T6>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expression, string tag = null)
        where T : BaseEntity
        where T2 : BaseEntity
        where T3 : BaseEntity
        where T4 : BaseEntity
        where T5 : BaseEntity
        where T6 : BaseEntity

        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7> ResoveWhereExpression<T, T2, T3, T4, T5, T6, T7>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expression, string tag = null)
      where T : BaseEntity
      where T2 : BaseEntity
      where T3 : BaseEntity
      where T4 : BaseEntity
      where T5 : BaseEntity
      where T6 : BaseEntity
            where T7 : BaseEntity

        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveWhereExpression<T, T2, T3, T4, T5, T6, T7, T8>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expression, string tag = null)
     where T : BaseEntity
     where T2 : BaseEntity
     where T3 : BaseEntity
     where T4 : BaseEntity
     where T5 : BaseEntity
     where T6 : BaseEntity
           where T7 : BaseEntity
            where T8 : BaseEntity

        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveWhereExpression<T, T2, T3, T4, T5, T6, T7, T8, T9>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expression, string tag = null)
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
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }


        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveWhereExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expression, string tag = null)
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
            var ps = ResoveExpression(expression.Body,tag:tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.WhereStr += string.IsNullOrEmpty(nt.WhereStr) ? $"({ps.SqlStr})" : $" and ({ps.SqlStr}) ";
            return nt;
        }

        #endregion

        #region Select
        internal static QingQuery<T> ResoveSelectExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, bool selectAlias = false,string tag=null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, selectAlias,tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T> ResoveSelectExceptExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, bool selectAlias, string tag = null) where T : BaseEntity
        {

            var ttype = typeof(T);
            var nea = Tools.GetNEA(ttype,tag);
            var func = (expression.Body as NewExpression);
            var properties = ttype.GetProperties();

            if (expression.Body.NodeType == ExpressionType.New)
            {
                List<string> exceptNames = new List<string>();
                foreach (var item in func.Arguments)
                {
                    exceptNames.Add((item as MemberExpression).Member.Name);
                }
                StringBuilder sb_select = new StringBuilder();
                foreach (var propertie in properties)
                {
                    if (!exceptNames.Contains(propertie.Name))
                    {
                        if (selectAlias)
                        {
                            var alias = Tools.GetFieldsAlias(propertie.DeclaringType, propertie.Name);

                            sb_select.Append($"{nea.TableName}.{nea._Lr}{propertie.Name}{nea._Rr} {Tools.AsAlias(alias)},");
                        }
                        else
                        {
                            sb_select.Append($"{nea.TableName}.{nea._Lr}{propertie.Name}{nea._Rr},");
                        }
                    }
                }
                nt.Fields += sb_select.ToString().TrimEnd(',');
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var name = (expression.Body as MemberExpression).Member.Name;
                StringBuilder sb_select = new StringBuilder();
                foreach (var propertie in properties)
                {
                    if (propertie.Name != name)
                    {
                        if (selectAlias)
                        {
                            var alias = Tools.GetFieldsAlias(propertie.DeclaringType, propertie.Name);
                            sb_select.Append($"{nea.TableName}.{nea._Lr}{propertie.Name}{nea._Rr} {Tools.AsAlias(alias)},");
                        }
                        else
                        {
                            sb_select.Append($"{nea.TableName}.{nea._Lr}{propertie.Name}{nea._Rr},");
                        }
                    }
                }
                nt.Fields += sb_select.ToString().TrimEnd(',');
            }
            else
            {
                SqlLogUtil.SendLog(LogType.Error, "SelectExcept 参数类型不支持");
            }
            return nt;
        }

        internal static QingQuery<T, T2> ResoveSelectExpression<T, T2, TResult>(QingQuery<T, T2> nt, Expression<Func<T, T2, TResult>> expression, bool selectAlias, string tag = null)
             where T : BaseEntity
           where T2 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }

        internal static QingQuery<T, T2, T3> ResoveSelectExpression<T, T2, T3, TResult>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, TResult>> expression, bool selectAlias, string tag = null)
              where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4> ResoveSelectExpression<T, T2, T3, T4, TResult>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, TResult>> expression, bool selectAlias, string tag = null)
             where T : BaseEntity
           where T2 : BaseEntity
           where T3 : BaseEntity
            where T4 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5> ResoveSelectExpression<T, T2, T3, T4, T5, TResult>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool selectAlias, string tag = null)
           where T : BaseEntity
         where T2 : BaseEntity
         where T3 : BaseEntity
          where T4 : BaseEntity
            where T5 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6> ResoveSelectExpression<T, T2, T3, T4, T5, T6, TResult>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool selectAlias, string tag = null)
         where T : BaseEntity
       where T2 : BaseEntity
       where T3 : BaseEntity
        where T4 : BaseEntity
          where T5 : BaseEntity
            where T6 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7> ResoveSelectExpression<T, T2, T3, T4, T5, T6, T7, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool selectAlias, string tag = null)
        where T : BaseEntity
      where T2 : BaseEntity
      where T3 : BaseEntity
       where T4 : BaseEntity
         where T5 : BaseEntity
           where T6 : BaseEntity
            where T7 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveSelectExpression<T, T2, T3, T4, T5, T6, T7, T8, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool selectAlias, string tag = null)
      where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
     where T4 : BaseEntity
       where T5 : BaseEntity
         where T6 : BaseEntity
          where T7 : BaseEntity
            where T8 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveSelectExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool selectAlias, string tag = null)
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

            var ps = ResoveExpression(expression.Body, selectAlias, tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveSelectExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool selectAlias, string tag = null)
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

            var ps = ResoveExpression(expression.Body,selectAlias,tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields += ps.SqlStr;
            return nt;
        }
        #endregion

        #region OrderBy

        internal static QingQuery<T> ResoveOrderByExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, bool Desc = false, string tag = null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body,tag:tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2> ResoveOrderByExpression<T, T2, TResult>(QingQuery<T, T2> nt, Expression<Func<T, T2, TResult>> expression, bool Desc = false, string tag = null)
           where T : BaseEntity
           where T2 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }
        internal static IQingQuery<T, T2, T3> ResoveOrderByExpression<T, T2, T3, TResult>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, TResult>> expression, bool Desc = false, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }
        internal static IQingQuery<T, T2, T3, T4> ResoveOrderByExpression<T, T2, T3, T4, TResult>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, TResult>> expression, bool Desc = false, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
            where T4 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5> ResoveOrderByExpression<T, T2, T3, T4, T5, TResult>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, TResult>> expression, bool Desc = false, string tag = null)
           where T : BaseEntity
           where T2 : BaseEntity
           where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6> ResoveOrderByExpression<T, T2, T3, T4, T5, T6, TResult>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, bool Desc = false, string tag = null)
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
          where T4 : BaseEntity
          where T5 : BaseEntity
            where T6 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7> ResoveOrderByExpression<T, T2, T3, T4, T5, T6, T7, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, bool Desc = false, string tag = null)
         where T : BaseEntity
         where T2 : BaseEntity
         where T3 : BaseEntity
         where T4 : BaseEntity
         where T5 : BaseEntity
           where T6 : BaseEntity
            where T7 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveOrderByExpression<T, T2, T3, T4, T5, T6, T7, T8, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, bool Desc = false, string tag = null)
        where T : BaseEntity
        where T2 : BaseEntity
        where T3 : BaseEntity
        where T4 : BaseEntity
        where T5 : BaseEntity
          where T6 : BaseEntity
           where T7 : BaseEntity
            where T8 : BaseEntity
        {

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveOrderByExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, bool Desc = false, string tag = null)
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

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveOrderByExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, bool Desc = false, string tag = null)
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

            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.OrderByStr += "," + ps.SqlStr + (Desc ? " Desc" : "");
            return nt;
        }
        #endregion

        #region Max
        internal static QingQuery<T> ResoveMaxExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, string tag = null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2> ResoveMaxExpression<T, T2, TResult>(QingQuery<T, T2> nt, Expression<Func<T, T2, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3> ResoveMaxExpression<T, T2, T3, TResult>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4> ResoveMaxExpression<T, T2, T3, T4, TResult>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5> ResoveMaxExpression<T, T2, T3, T4, T5, TResult>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6> ResoveMaxExpression<T, T2, T3, T4, T5, T6, TResult>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7> ResoveMaxExpression<T, T2, T3, T4, T5, T6, T7, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveMaxExpression<T, T2, T3, T4, T5, T6, T7, T8, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveMaxExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity where T9 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveMaxExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity where T9 : BaseEntity where T10 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body,tag:tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Max({ps.SqlStr})";
            return nt;
        }
        #endregion

        #region Min
        internal static QingQuery<T> ResoveMinExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, string tag = null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2> ResoveMinExpression<T, T2, TResult>(QingQuery<T, T2> nt, Expression<Func<T, T2, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3> ResoveMinExpression<T, T2, T3, TResult>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4> ResoveMinExpression<T, T2, T3, T4, TResult>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5> ResoveMinExpression<T, T2, T3, T4, T5, TResult>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6> ResoveMinExpression<T, T2, T3, T4, T5, T6, TResult>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7> ResoveMinExpression<T, T2, T3, T4, T5, T6, T7, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }

        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveMinExpression<T, T2, T3, T4, T5, T6, T7, T8, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveMinExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity where T9 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        internal static QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveMinExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, string tag = null) where T : BaseEntity where T2 : BaseEntity where T3 : BaseEntity where T4 : BaseEntity where T5 : BaseEntity where T6 : BaseEntity where T7 : BaseEntity where T8 : BaseEntity where T9 : BaseEntity where T10 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.Fields = $"Min({ps.SqlStr})";
            return nt;
        }
        #endregion

        #region Join
        internal static ParamSql ResoveJoinExpression<T, T2>(QingQuery<T> nt, Expression<Func<T, T2, bool>> expressionJoinOn, string tag = null)
       where T : BaseEntity
       where T2 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3>(QingQuery<T, T2> nt, Expression<Func<T, T2, T3, bool>> expressionJoinOn, string tag = null)
        where T : BaseEntity
        where T2 : BaseEntity
        where T3 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }
        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, T4, bool>> expressionJoinOn, string tag = null)
       where T : BaseEntity
       where T2 : BaseEntity
       where T3 : BaseEntity
            where T4 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, T5, bool>> expressionJoinOn, string tag = null)
      where T : BaseEntity
      where T2 : BaseEntity
      where T3 : BaseEntity
           where T4 : BaseEntity
            where T5 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5, T6>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, T6, bool>> expressionJoinOn, string tag = null)
     where T : BaseEntity
     where T2 : BaseEntity
     where T3 : BaseEntity
          where T4 : BaseEntity
           where T5 : BaseEntity
            where T6 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }
        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5, T6, T7>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, bool>> expressionJoinOn, string tag = null)
    where T : BaseEntity
    where T2 : BaseEntity
    where T3 : BaseEntity
         where T4 : BaseEntity
          where T5 : BaseEntity
           where T6 : BaseEntity
            where T7 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5, T6, T7, T8>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, bool>> expressionJoinOn, string tag = null)
   where T : BaseEntity
   where T2 : BaseEntity
   where T3 : BaseEntity
        where T4 : BaseEntity
         where T5 : BaseEntity
          where T6 : BaseEntity
           where T7 : BaseEntity
            where T8 : BaseEntity
        {
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5, T6, T7, T8, T9>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expressionJoinOn, string tag = null)
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
            return ResoveExpression(expressionJoinOn.Body, tag: tag);
        }

        internal static ParamSql ResoveJoinExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expressionJoinOn, string tag = null)
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
            return ResoveExpression(expressionJoinOn.Body,tag:tag);
        }
        #endregion

        #region GroupBy
        internal static IQingQuery<T> ResoveGroupByExpression<T, TResult>(QingQuery<T> nt, Expression<Func<T, TResult>> expression, string tag = null) where T : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T, T2> ResoveGroupByExpression<T, T2, TResult>(QingQuery<T, T2> nt, Expression<Func<T, T2, TResult>> expression, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }
        internal static IQingQuery<T, T2, T3> ResoveGroupByExpression<T, T2, T3, TResult>(QingQuery<T, T2, T3> nt, Expression<Func<T, T2, T3, TResult>> expression, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4> ResoveGroupByExpression<T, T2, T3, T4, TResult>(QingQuery<T, T2, T3, T4> nt, Expression<Func<T, T2, T3, T4, TResult>> expression, string tag = null)
            where T : BaseEntity
            where T2 : BaseEntity
            where T3 : BaseEntity
            where T4 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }
        internal static IQingQuery<T, T2, T3, T4, T5> ResoveGroupByExpression<T, T2, T3, T4, T5, TResult>(QingQuery<T, T2, T3, T4, T5> nt, Expression<Func<T, T2, T3, T4, T5, TResult>> expression, string tag = null)
           where T : BaseEntity
           where T2 : BaseEntity
           where T3 : BaseEntity
           where T4 : BaseEntity
           where T5 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6> ResoveGroupByExpression<T, T2, T3, T4, T5, T6, TResult>(QingQuery<T, T2, T3, T4, T5, T6> nt, Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression, string tag = null)
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
          where T4 : BaseEntity
          where T5 : BaseEntity
         where T6 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7> ResoveGroupByExpression<T, T2, T3, T4, T5, T6, T7, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression, string tag = null)
          where T : BaseEntity
          where T2 : BaseEntity
          where T3 : BaseEntity
          where T4 : BaseEntity
          where T5 : BaseEntity
         where T6 : BaseEntity
            where T7 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }

        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8> ResoveGroupByExpression<T, T2, T3, T4, T5, T6, T7, T8, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, string tag = null)
         where T : BaseEntity
         where T2 : BaseEntity
         where T3 : BaseEntity
         where T4 : BaseEntity
         where T5 : BaseEntity
        where T6 : BaseEntity
           where T7 : BaseEntity
            where T8 : BaseEntity
        {
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }
        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> ResoveGroupByExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, string tag = null)
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
            var ps = ResoveExpression(expression.Body, tag: tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }
        internal static IQingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> ResoveGroupByExpression<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(QingQuery<T, T2, T3, T4, T5, T6, T7, T8, T9, T10> nt, Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, string tag = null)
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
            var ps = ResoveExpression(expression.Body,tag:tag);
            nt.SqlParams.AddDynamicParams(ps.Params);
            nt.GroupByStr = ps.SqlStr;
            return nt;
        }
        #endregion


        #region Update
        internal static List<ParamSql> ResoveUpdateExpression<T, TResult>(Expression<Func<T, TResult>> expression, string tag = null) where T : BaseEntity
        {
            var nea = Tools.GetNEA(typeof(T),tag);
            if (expression.Body.NodeType == ExpressionType.MemberInit)
            {
                var func = (MemberInitExpression)expression.Body;
                List<ParamSql> pss = new List<ParamSql>();
                foreach (var item in func.Bindings)
                {
                    var assignment = (MemberAssignment)item;
                    var key = assignment.Member.Name;
                    var ps = ResoveExpression(assignment.Expression,tag:tag);
                    ps.SqlStr = $"{nea._Lr}{key}{nea._Rr}={ps.SqlStr}";
                    pss.Add(ps);
                }
                return pss;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var func = (MemberExpression)expression.Body;
                List<ParamSql> pss = new List<ParamSql>();
                var properties = func.Type.GetProperties();
                foreach (var propertiy in properties)
                {
                    ParamSql _ps = new ParamSql();

                    var key = Tools.RandomCode(6, 0, true);
                    _ps.SqlStr = $"{propertiy.Name} = @{key}";

                    var constant = (ConstantExpression)func.Expression;
                    var fieldInfoValue = ((FieldInfo)func.Member).GetValue(constant.Value);
                    var value = propertiy.GetValue(fieldInfoValue, null);
                    switch (propertiy.PropertyType.Name)
                    {

                        case "String":
                            {
                                _ps.Params.Add(key, value, System.Data.DbType.String);
                            }
                            break;
                        case "Int32":
                            {
                                _ps.Params.Add(key, value, System.Data.DbType.Int32);

                            }
                            break;
                        case "Decimal":
                            {
                                _ps.Params.Add(key, value, System.Data.DbType.Decimal);

                            }
                            break;
                        case "DateTime":
                            if ((DateTime)value == DateTime.MinValue && nea._dbType == "MsSql")
                            {
                                value = null;
                            }
                            _ps.Params.Add(key, value, System.Data.DbType.DateTime);
                            break;
                        case "Int64":
                            {
                                _ps.Params.Add(key, value, System.Data.DbType.Int64);
                            }
                            break;
                        default:
                            {
                                _ps.Params.Add(key, value, System.Data.DbType.String);
                            }
                            break;
                    }
                    pss.Add(_ps);
                }
                return pss;
            }
            return null;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 通过Lambda解析为Sql
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        internal static ParamSql ResoveExpression(Expression func, bool selectAlias = false,string tag=null)
        {
            ParamSql result = new ParamSql();
            switch (func.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Equal:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.NotEqual:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Negate:
                case ExpressionType.Subtract:
                case ExpressionType.Divide:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:

                    result = BinaryExpression(func as BinaryExpression, tag);
                    break;
                case ExpressionType.Constant:
                    result = ConstantExpression(func as ConstantExpression, tag);
                    break;
                case ExpressionType.Call:

                    result = MethodCallExpression(func as MethodCallExpression, tag);
                    break;
                case ExpressionType.Quote:
                case ExpressionType.Not:
                case ExpressionType.Convert:

                    result = UnaryExpression(func as UnaryExpression);
                    break;
                case ExpressionType.MemberAccess:

                    result = MemberAccessExpression(func as MemberExpression, selectAlias, tag);
                    break;
                case ExpressionType.New:
                    result = NewExpression(func as NewExpression, selectAlias,tag);
                    break;
                case ExpressionType.MemberInit:
                    result = MemberInitExpression(func as MemberInitExpression, tag);
                    break;
                case ExpressionType.Lambda:
                    result = ResoveExpression((func as LambdaExpression).Body, tag:tag);
                    break;
                case ExpressionType.NewArrayInit:
                    result = NewArrayExpression(func as NewArrayExpression, tag);
                    break;
                #region 暂不支持&不会识别的类型
                case ExpressionType.And:
                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Invoke:
                case ExpressionType.LeftShift:
                case ExpressionType.ListInit:
                case ExpressionType.Modulo:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Or:
                case ExpressionType.Parameter:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.SubtractChecked:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                case ExpressionType.Assign:
                case ExpressionType.Block:
                case ExpressionType.DebugInfo:
                case ExpressionType.Decrement:
                case ExpressionType.Dynamic:
                case ExpressionType.Default:
                case ExpressionType.Extension:
                case ExpressionType.Goto:
                case ExpressionType.Increment:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.RuntimeVariables:
                case ExpressionType.Loop:
                case ExpressionType.Switch:
                case ExpressionType.Throw:
                case ExpressionType.Try:
                case ExpressionType.Unbox:
                case ExpressionType.AddAssign:
                case ExpressionType.AndAssign:
                case ExpressionType.DivideAssign:
                case ExpressionType.ExclusiveOrAssign:
                case ExpressionType.LeftShiftAssign:
                case ExpressionType.ModuloAssign:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.OrAssign:
                case ExpressionType.PowerAssign:
                case ExpressionType.RightShiftAssign:
                case ExpressionType.SubtractAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                case ExpressionType.PostIncrementAssign:
                case ExpressionType.PostDecrementAssign:
                case ExpressionType.TypeEqual:
                case ExpressionType.OnesComplement:
                case ExpressionType.IsTrue:
                case ExpressionType.IsFalse:
                    #endregion
                    break;
            }
            return result;
        }

        /// <summary>
        /// 判断一元表达式
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static ParamSql UnaryExpression(UnaryExpression func, string tag = null)
        {
            ParamSql ps = new ParamSql();
            var result = GetOperator(func.NodeType);
            var psr = ResoveExpression(func.Operand, tag:tag);
            if (result == "Not" && psr.SqlStr.Contains("like"))
            {
                ps.SqlStr = psr.SqlStr.Replace(" like ", " Not like ");
                ps.Params.AddDynamicParams(psr.Params);
            }
            else
            {
                ps.SqlStr = result + psr.SqlStr;
                ps.Params.AddDynamicParams(psr.Params);
            }

            return ps;
        }


        private static ParamSql NewExpression(NewExpression func, bool selectAlias = false, string tag=null)
        {
            ParamSql ps = new ParamSql();
            List<string> sqls = new List<string>();
            for (int i = 0; i < func.Arguments.Count; i++)
            {
                var arg = func.Arguments[i];
                var member = func.Members[i];
                if (arg.NodeType == ExpressionType.MemberAccess)
                {
                    var pr = ResoveExpression(arg, selectAlias,tag: tag);
                    ps.Params.AddDynamicParams(pr.Params);
                    if (((MemberExpression)arg).Member.Name != member.Name)
                    {
                        pr.SqlStr += $" as '{member.Name}'";
                    }
                    sqls.Add(pr.SqlStr);
                }
                else if (arg.NodeType == ExpressionType.Call)
                {

                    var argsql = ResoveExpression(arg, selectAlias, tag: tag);
                    ps.Params.AddDynamicParams(argsql.Params);
                    if (argsql.SqlStr.EndsWith("*"))
                    {
                        sqls.Add($"{argsql.SqlStr}");
                    }
                    else
                    {
                        sqls.Add($"{argsql.SqlStr} as '{member.Name}'");
                    }

                }
                else
                {
                    sqls.Add($"{arg} as '{member.Name}'");
                }
            }
            ps.SqlStr = string.Join(",", sqls);
            return ps;
        }


        private static ParamSql MemberInitExpression(MemberInitExpression func, string tag)
        {
            ParamSql ps = new ParamSql();
            List<string> list_str = new List<string>();

            //foreach (var item in func.Bindings)
            //{
            //    var assignment = (MemberAssignment)item;

            //    var key = assignment.Member.Name;
            //    var value = ResoveExpression(assignment.Expression);
            //    list_str.Add($"{key} = @{key}");

            //    ps.Params.Add(key, value);
            //}

            foreach (var item in func.Bindings)
            {
                var assignment = (MemberAssignment)item;
                var sql = ResoveExpression(assignment.Expression,tag:tag);
                list_str.Add($"{sql.SqlStr} as '{assignment.Member.Name}'");
            }
            ps.SqlStr = string.Join(",", list_str);
            return ps;
        }

        private static ParamSql NewArrayExpression(NewArrayExpression func, string tag)
        {
            ParamSql ps = new ParamSql();

            foreach (var item in func.Expressions)
            {
                var _ps = ResoveExpression(item,tag: tag);
                ps.SqlStr += _ps.SqlStr + ",";
                ps.Params.AddDynamicParams(_ps.Params);
            }
            ps.SqlStr = ps.SqlStr.TrimEnd(',');
            return ps;
        }

        private static ParamSql BinaryExpression(BinaryExpression func, string tag)
        {
            ParamSql ps = new ParamSql();
            var lps = ResoveExpression(func.Left,tag:tag);
            var rps = ResoveExpression(func.Right, tag: tag);
            var _operator = GetOperator(func.NodeType);
            if (rps?.SqlStr?.Trim() == "NULL")
            {
                if (_operator.Trim() == "=")
                {
                    _operator = " IS ";
                }
                else if (_operator.Trim() == "<>")
                {
                    _operator = " IS NOT ";
                }
            }
            if (func.NodeType == ExpressionType.AndAlso)
            {
                ps.SqlStr = $"({lps.SqlStr}) {_operator} ({rps.SqlStr})";
            }
            else
            {
                ps.SqlStr = $"{lps.SqlStr} {_operator} {rps.SqlStr}";
            }
            ps.Params.AddDynamicParams(lps.Params);
            ps.Params.AddDynamicParams(rps.Params);
            return ps;
        }

        private static ParamSql ConstantExpression(ConstantExpression func, string tag)
        {
            ParamSql ps = new ParamSql();
            if (func.Value == null)
            {
                ps.SqlStr = "NULL";
            }
            else if (func.Value.ToString() == "")
            {
                ps.SqlStr = "\'\' ";
            }
            else if (func.Value.ToString() == "True")
            {
                ps.SqlStr = "1 = 1 ";
            }
            else if (func.Value.ToString() == "False")
            {
                ps.SqlStr = "0 = 1 ";
            }
            else
            {

                string placeholder = Tools.RandomCode(6, 0, true);
                ps.SqlStr = "@" + placeholder;
                if (func.Type.Name == "DateTime")
                {
                    ps.Params.Add(placeholder, func.Value, System.Data.DbType.DateTime);
                }
                else
                {
                    ps.Params.Add(placeholder, func.Value);
                }
            }

            return ps;
        }



        /// <summary>
        /// 判断包含变量的表达式
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static ParamSql MemberAccessExpression(MemberExpression func, bool selectAlias = false, string tag = null)
        {
            ParamSql ps = new ParamSql();

            if (func.Expression != null && func.Expression.NodeType == ExpressionType.Parameter)
            {
                if (func.Member.DeclaringType.BaseType == typeof(BaseEntity))
                {
                    var nea = Tools.GetNEA(func.Expression.Type,tag);
                    if (selectAlias)
                    {


                        var alias = Tools.GetFieldsAlias(func.Member.DeclaringType, func.Member.Name);
                        ps.SqlStr = $"{nea.TableName}.{nea._Lr}{func.Member.Name}{nea._Rr} {Tools.AsAlias(alias)}";
                    }
                    else
                    {
                        ps.SqlStr = $"{nea.TableName}.{nea._Lr}{func.Member.Name}{nea._Rr}";
                    }
                }
                else
                {
                    ps.SqlStr = $"{Tools.Lr(tag)}{func.Member.Name}{Tools.Rr(tag)}";
                }
            }
            else if (func.Expression != null && func.Expression.NodeType == ExpressionType.Call)
            {

                var res = ResoveExpression(func.Expression);
            }

            else
            {

                switch (func.Type.Name)
                {

                    case "String":
                        {
                            var getter = Expression.Lambda<Func<string>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter(), System.Data.DbType.String);
                        }
                        break;
                    case "Int32":
                        {
                            var getter = Expression.Lambda<Func<int>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter(), System.Data.DbType.Int32);

                        }
                        break;
                    case "Decimal":
                        {
                            var getter = Expression.Lambda<Func<decimal>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter(), System.Data.DbType.Decimal);

                        }
                        break;
                    case "DateTime":
                        {
                            var getter = Expression.Lambda<Func<DateTime>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter(), System.Data.DbType.DateTime);
                        }
                        break;
                    case "Int64":
                        {
                            var getter = Expression.Lambda<Func<long>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter(), System.Data.DbType.Int64);

                        }
                        break;
                    case "String[]":
                        {
                            var getter = Expression.Lambda<Func<string[]>>(func).Compile();
                            var arrayData = getter();
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in arrayData)
                            {
                                var key = Tools.RandomCode(6, 0, true);
                                sb.Append($"@{key},");
                                ps.Params.Add(key, item, System.Data.DbType.String);
                            }
                            ps.SqlStr = sb.ToString().TrimEnd(',');


                        }
                        break;
                    case "Int32[]":
                        {
                            var getter = Expression.Lambda<Func<int[]>>(func).Compile();
                            var arrayData = getter();
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in arrayData)
                            {
                                var key = Tools.RandomCode(6, 0, true);
                                sb.Append($"@{key},");
                                ps.Params.Add(key, item, System.Data.DbType.Int32);
                            }
                            ps.SqlStr = sb.ToString().TrimEnd(',');
                        }
                        break;
                    case "Int64[]":
                        {
                            var getter = Expression.Lambda<Func<long[]>>(func).Compile();
                            var arrayData = getter();
                            StringBuilder sb = new StringBuilder();

                            foreach (var item in arrayData)
                            {
                                var key = Tools.RandomCode(6, 0, true);
                                sb.Append($"@{key},");
                                ps.Params.Add(key, item, System.Data.DbType.Int64);
                            }
                            ps.SqlStr = sb.ToString().TrimEnd(',');
                        }
                        break;
                    case "Decimal[]":
                        {
                            var getter = Expression.Lambda<Func<decimal[]>>(func).Compile();
                            var arrayData = getter();
                            StringBuilder sb = new StringBuilder();
                            foreach (var item in arrayData)
                            {
                                var key = Tools.RandomCode(6, 0, true);
                                sb.Append($"@{key},");
                                ps.Params.Add(key, item, System.Data.DbType.Decimal);
                            }
                            ps.SqlStr = sb.ToString().TrimEnd(',');

                        }
                        break;
                    default:
                        {
                            var getter = Expression.Lambda<Func<object>>(func).Compile();
                            var key = Tools.RandomCode(6, 0, true);
                            ps.SqlStr = "@" + key;
                            ps.Params.Add(key, getter().ToString(), System.Data.DbType.String);
                        }
                        break;
                }
            }
            return ps;
        }


        /// <summary>
        /// 判断包含函数的表达式
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static ParamSql MethodCallExpression(MethodCallExpression func,string tag=null)
        {
            ParamSql ps = new ParamSql();
            string methodName = func.Method.Name;
            switch (methodName)
            {
                case "Contains":
                    {
                        var field_ps = ResoveExpression(func.Object);
                        ps.Params.AddDynamicParams(field_ps.Params);
                        var value_ps = ResoveExpression(func.Arguments[0]);
                        string key = value_ps.SqlStr.TrimStart('@');
                        string value = Tools.SafeKeyWord(value_ps.Params.Get<string>(key));
                        ps.Params.Add(key, value);
                        ps.SqlStr = $"{field_ps.SqlStr} like  concat('%',{value_ps.SqlStr},'%')";
                    }
                    break;
                case "StartsWith":
                    {


                        var field_ps = ResoveExpression(func.Object);
                        ps.Params.AddDynamicParams(field_ps.Params);
                        var value_ps = ResoveExpression(func.Arguments[0]);
                        string key = value_ps.SqlStr.TrimStart('@');
                        string value = Tools.SafeKeyWord(value_ps.Params.Get<string>(key));
                        ps.Params.Add(key, value);
                        ps.SqlStr = $"{field_ps.SqlStr} like concat({value_ps.SqlStr},'%')";
                    }
                    break;
                case "EndsWith":
                    {
                        var field_ps = ResoveExpression(func.Object);
                        ps.Params.AddDynamicParams(field_ps.Params);
                        var value_ps = ResoveExpression(func.Arguments[0]);
                        string key = value_ps.SqlStr.TrimStart('@');
                        string value = Tools.SafeKeyWord(value_ps.Params.Get<string>(key));
                        ps.Params.Add(key, value);
                        ps.SqlStr = $"{field_ps.SqlStr} like  concat('%',{value_ps.SqlStr})";
                    }
                    break;
                case "Equals":
                    {
                        var field_PS = ResoveExpression(func.Object);
                        var value_PS = ResoveExpression(func.Arguments[0]);
                        ps.Params.AddDynamicParams(field_PS.Params);
                        ps.Params.AddDynamicParams(value_PS.Params);
                        ps.SqlStr = $"{field_PS.SqlStr} = {value_PS.SqlStr}";
                    }
                    break;
                case "IsNullOrEmpty":
                    {
                        var field_PS = ResoveExpression(func.Arguments[0]);
                        ps.Params.AddDynamicParams(field_PS.Params);
                        ps.SqlStr = $"{field_PS.SqlStr} IS NULL or {field_PS.SqlStr} =''";
                    }
                    break;


                default:

                    if (func.Type == typeof(DateTime))
                    {
                        var invokeResult = Expression.Lambda<Func<DateTime>>(func).Compile().DynamicInvoke();
                        var key = Tools.RandomCode(6, 0, true);
                        ps.SqlStr = "@" + key;
                        ps.Params.Add(key, invokeResult, System.Data.DbType.DateTime);
                    }
                    else
                    {
                        if (func.Method.ReflectedType.FullName == "Qing.DB.Sqm")
                        {
                            switch (func.Method.Name)
                            {
                                case "Count":
                                    var countsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"COUNT({countsql.SqlStr})";
                                    break;
                                case "Abs":
                                    var abssql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"ABS({abssql.SqlStr})";
                                    break;
                                case "Min":
                                    var minsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"MIN({minsql.SqlStr})";
                                    break;
                                case "Max":
                                    var maxsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"MAX({maxsql.SqlStr})";
                                    break;
                                case "Avg":
                                    var avgsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"AVG({avgsql.SqlStr})";
                                    break;
                                case "Sum":
                                    var sumsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"SUM({sumsql.SqlStr})";
                                    break;
                                case "Length":
                                    var lengthsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"LENGTH({lengthsql.SqlStr})";
                                    break;
                                case "Distinct":
                                    var distinctsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"DISTINCT({distinctsql.SqlStr})";
                                    break;
                                case "Concat":
                                    var concatsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"CONCAT({concatsql.SqlStr})";
                                    break;
                                case "Concat_ws":
                                    var spsql = ResoveExpression(func.Arguments[0]);
                                    var concatwssql = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(spsql.Params);
                                    ps.SqlStr = $"CONCAT_WS({spsql.SqlStr},{concatwssql.SqlStr})";
                                    break;
                                case "Format":
                                    var formatsql = ResoveExpression(func.Arguments[0]);
                                    var numSql = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(formatsql.Params);
                                    ps.Params.AddDynamicParams(numSql.Params);
                                    ps.SqlStr = $"FORMAT({formatsql.SqlStr},{numSql.SqlStr})";
                                    break;
                                case "GroupConcat":
                                    var groupConcatsql = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"GROUP_CONCAT({groupConcatsql.SqlStr})";
                                    break;
                                case "GroupConcat_ws":
                                    var groupConcatsql2 = ResoveExpression(func.Arguments[0]);
                                    var groupConcatspr = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(groupConcatspr.Params);
                                    ps.SqlStr = $"GROUP_CONCAT({groupConcatsql2.SqlStr}  separator {groupConcatspr.SqlStr})";
                                    break;
                                case "IFNULL":
                                    var arg1 = ResoveExpression(func.Arguments[0]);
                                    var arg2 = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(arg2.Params);
                                    ps.SqlStr = $"IFNULL({arg1.SqlStr},{arg2.SqlStr})";
                                    break;
                                case "In":
                                    var in_arg1 = ResoveExpression(func.Arguments[0]);
                                    var invalues = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(invalues.Params);
                                    ps.SqlStr = $"{in_arg1.SqlStr} In ({invalues.SqlStr})";
                                    break;
                                case "NotIn":
                                    var notin_arg1 = ResoveExpression(func.Arguments[0]);
                                    var notinvalues = ResoveExpression(func.Arguments[1]);
                                    ps.Params.AddDynamicParams(notinvalues.Params);
                                    ps.SqlStr = $"{notin_arg1.SqlStr} Not In ({notinvalues.SqlStr})";
                                    break;
                                case "IsNullOrEmpty":
                                    var isNullOrEmpty_arg = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"{isNullOrEmpty_arg.SqlStr} IS NULL OR {isNullOrEmpty_arg.SqlStr} =''";
                                    break;
                                case "IsNotNullOrEmpty":
                                    var isNotNullOrEmpty_arg = ResoveExpression(func.Arguments[0]);
                                    ps.SqlStr = $"{isNotNullOrEmpty_arg.SqlStr} IS NOT NULL AND {isNotNullOrEmpty_arg.SqlStr} <>''";
                                    break;
                                case "TableAll":
                                    var nea = Tools.GetNEA(func.Arguments[0].Type);
                                    ps.SqlStr = $"{nea.TableName}.*";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            var result = Expression.Lambda(func).Compile().DynamicInvoke();

                            if (func.Type == typeof(ParamSql))
                            {
                                ps = (ParamSql)result;
                            }
                            else if (func.Type.BaseType == typeof(Array))
                            {
                                var arrayResult = (Array)result;

                                string sqlStr = "";
                                foreach (var item in arrayResult)
                                {
                                    var key = Tools.RandomCode(6, 0, true);
                                    sqlStr += ",@" + key;
                                    ps.Params.Add(key, item);
                                }
                                ps.SqlStr = sqlStr.TrimStart(',');
                            }
                            else
                            {
                                var key = Tools.RandomCode(6, 0, true);
                                ps.SqlStr = "@" + key;
                                ps.Params.Add(key, result);
                            }
                        }
                    }

                    break;
            }

            return ps;
        }


        /// <summary>
        /// 生成操作符
        /// </summary>
        /// <param name="expressiontype"></param>
        /// <returns></returns>
        private static string GetOperator(ExpressionType expressiontype)
        {
            switch (expressiontype)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " OR ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Not:
                    return "Not";
                default:
                    return "";
            }
        }

        private static bool IsNumberType(this Type dataType)
        {
            if (dataType == null)
                return false;

            return (dataType == typeof(int)
                    || dataType == typeof(double)
                    || dataType == typeof(Decimal)
                    || dataType == typeof(long)
                    || dataType == typeof(short)
                    || dataType == typeof(float)
                    || dataType == typeof(Int16)
                    || dataType == typeof(Int32)
                    || dataType == typeof(Int64)
                    || dataType == typeof(uint)
                    || dataType == typeof(UInt16)
                    || dataType == typeof(UInt32)
                    || dataType == typeof(UInt64)
                    || dataType == typeof(Single)
                   );
        }
        #endregion

    }


}
