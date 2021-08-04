using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Qing.DB
{
    public static class List_Extenson
    {
        public static IEnumerable<T> QueryWithPage<T>(this IEnumerable<T> ts, int pageNum, int pageSize)
        {
            pageNum = pageNum <= 0 ? 1 : pageNum;
            pageSize = pageSize <= 0 ? 1 : pageSize;

            return ts?.Skip((pageNum - 1) * pageSize)?.Take(pageSize);
        }

        public static IEnumerable<T> DoSort<T>(this IEnumerable<T> ts, Expression<Func<T, object>> expression, bool desc = false) where T : BaseEntity
        {
            if (expression != null)
            {
                if (!desc)
                {
                    ts = ts.OrderBy(expression.Compile()).ToList();
                }
                else
                {
                    ts = ts.OrderByDescending(expression.Compile()).ToList();
                }
            }
            return ts;
        }
    }
}
