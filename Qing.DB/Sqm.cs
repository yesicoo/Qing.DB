using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    public static class Sqm
    {
        public static double Abs(object f)
        {
            return 0;
        }
        public static int Count(object f)
        {
            return 0;
        }
        public static object IFNULL(object value1, object defaultvalue)
        {
            return null;
        }

        public static double Min(object f)
        {
            return 0;
        }
        public static double Max(object f)
        {
            return 0;
        }
        public static double Avg(object f)
        {
            return 0;
        }
        public static double Sum(object f)
        {
            return 0;
        }
        public static int Length(object f)
        {
            return 0;
        }

        public static object Distinct(object f)
        {
            return null;
        }

        public static object Concat(params object[] f)
        {
            return null;
        }

        /// <summary>
        /// 字段连接
        /// </summary>
        /// <param name="spStr">间隔字符</param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static object Concat_ws(string spStr, params object[] f)
        {
            return null;
        }

        public static object GroupConcat(object f)
        {
            return null;
        }
        public static object GroupConcat_ws(object f, string spr)
        {
            return null;
        }

        public static bool In(object f, params object[] value)
        {
            return true;
        }
        public static bool In(object f, ParamSql QingQuery)
        {
            return true;
        }
        public static bool NotIn(object f, params object[] value)
        {
            return true;
        }
        //public static bool NotIn(object f, ParamSql QingQuery)
        //{
        //    return true;
        //}
        public static bool IsNullOrEmpty(object f)
        {
            return true;
        }
        public static bool IsNotNullOrEmpty(object f)
        {
            return true;
        }

        /// <summary>
        /// Table.* 全部字段的意思
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public static object TableAll<T>(T x) where T : BaseEntity
        {
            return null;
        }
        public static object Format(object f, int num)
        {
            return null;
        }
    }
}
