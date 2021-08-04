using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qing.DB
{
    internal class Tools
    {
        internal static string Lr(string tag)
        {
            var dbType = DBConnPools.DBType(tag);
            if (dbType == "MsSql")
            {
                return "[";
            }
            else
            {
                return "`";
            }
        }
        internal static string Rr(string tag)
        {
            var dbType = DBConnPools.DBType(tag);
            if (dbType == "MsSql")
            {
                return "]";
            }
            else
            {
                return "`";
            }
        }
        static ConcurrentDictionary<Type, QingEntityAttribute> _neas = new ConcurrentDictionary<Type, QingEntityAttribute>();
        #region 随机生成字符串
        /// <summary>
        /// 随机生成字符串
        /// </summary>
        /// <param name="num">位数</param>
        /// <param name="type">
        /// 0 -区分大小写字符包含数字的随机字符串
        /// 1 -小写字符含数字字符串
        /// 2 -大写字符包含数字字符串
        /// 3 -小写字符随机字符串
        /// 4 -大写字符随机字符串
        /// 5 -仅数字
        /// </param>
        /// <returns></returns>
        public static string RandomCode(int num, int type = 0, bool firstUpper = false)
        {
            string chars = null;

            switch (type)
            {
                case 0:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghicklnopqrstuvwxyz1234567890";
                    break;
                case 1:
                case 2:
                    chars = "abcdefghicklnopqrstuvwxyz1234567890";
                    break;
                case 3:
                case 4:
                    chars = "abcdefghicklnopqrstuvwxyz";
                    break;
                case 5:
                    chars = "1234567890";
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(chars))
            {
                int length = chars.Length;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < num; i++)
                {
                    Random r = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                    if (firstUpper && i == 0)
                    {
                        sb.Append("abcdefghicklnopqrstuvwxyz"[r.Next(0, 25)]);
                    }
                    else
                    {
                        sb.Append(chars[r.Next(0, length - 1)]);
                    }
                }
                var result = sb.ToString();
                if (type == 2 || type == 4)
                {
                    result = result.ToUpper();
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        internal static string GetDBID(string dbid)
        {
            return string.IsNullOrEmpty(dbid) ? DBFactory.Instance.DefaultDBID() : dbid;
        }
        #endregion


        internal static Dictionary<string, string> GetFieldsAlias(Type dataType)
        {
            Dictionary<string, string> propertiy_attribute = new Dictionary<string, string>();
            var properties = dataType.GetProperties();
            foreach (var propertiy in properties)
            {
                var epa = (QingPropertyAttribute)Attribute.GetCustomAttribute(propertiy, typeof(QingPropertyAttribute));
                propertiy_attribute.Add(propertiy.Name, epa.Alias);
            }
            return propertiy_attribute;
        }

        internal static string GetFieldsAlias(Type dataType, string fieldName)
        {
            var propertiy = dataType.GetProperty(fieldName);
            if (propertiy != null)
            {
                var epa = (QingPropertyAttribute)Attribute.GetCustomAttribute(propertiy, typeof(QingPropertyAttribute));
                return epa?.Alias;
            }
            else
            {
                return null;
            }
        }

        internal static T TimeoutCheck<T>(int ms, Func<T> func)
        {
            T result = default(T);
            Task.Run(() =>
            {
                try
                {
                    result = func.Invoke();
                }
                catch (Exception ex)
                {
                    SqlLogUtil.SendLog(LogType.Error, ex.ToString());
                }
            }).Wait(ms);

            return result;
        }

        internal static string SafeKeyWord(string keyword)
        {
            return keyword.Replace("\\", "\\\\").Replace("%", "\\%").Replace("'", "\\'").Replace("_", "\\_");
        }

        internal static QingEntityAttribute GetNEA(Type type,string tag=null)
        {

            if (!_neas.TryGetValue(type, out var nea))
            {
                nea = (QingEntityAttribute)Attribute.GetCustomAttribute(type, typeof(QingEntityAttribute));
                _neas.TryAdd(type, nea);
            }
            nea._dbType = DBConnPools.DBType(tag);
            if (nea._dbType == "MsSql")
            {
                nea._Lr = "[";
                nea._Rr = "]";
            }
            else
            {
                nea._Lr = "`";
                nea._Rr = "`";
            }
            return nea;

          
        }

     

        internal static string AsAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return string.Empty;
            }
            else
            {
                return $"as '{alias}'";
            }
        }

        internal static string AppendTableSuffix(QingEntityAttribute nea, string sql, string tableSuffix)
        {
            string tasbSuffixStr = string.Empty;
            if (tableSuffix.Contains("["))
            {
                if (tableSuffix.Contains("[DAY]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[DAY]", DateTime.Now.ToString("yyyyMMdd"));
                }
                else if (tableSuffix.Contains("[MONTH]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[MONTH]", DateTime.Now.ToString("yyyyMM"));
                }
                else if (tableSuffix.Contains("[YEAR]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[YEAR]", DateTime.Now.ToString("yyyy"));
                }
                else if (tableSuffix.Contains("[HOUR]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[HOUR]", DateTime.Now.ToString("yyyyMMddHH"));
                }
                else if (tableSuffix.Contains("[WEEK]"))
                {
                    DateTime dt = DateTime.Today;
                    int count = dt.DayOfWeek - DayOfWeek.Monday;
                    if (count == -1) count = 6;
                    string res = dt.AddDays(-count).ToString("yyyyMMdd");

                    tasbSuffixStr = tableSuffix.Replace("[WEEK]", res);
                }
                else if (tableSuffix.Contains("[WEEK7]"))
                {
                    string res = "";
                    DateTime dt = DateTime.Today;
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                        res = dt.ToString("yyyyMMdd");
                    else
                        res = dt.AddDays(7 - (dt.DayOfWeek - DayOfWeek.Sunday)).ToString("yyyyMMdd");
                    tasbSuffixStr = tableSuffix.Replace("[WEEK7]", res);
                }
                else
                {
                    tasbSuffixStr = tableSuffix;
                }
            }
            else
            {
                tasbSuffixStr = tableSuffix;
            }
            if (nea._dbType == "MsSql")
            {
                tasbSuffixStr = $"[dbo].[{nea._tableName}{tasbSuffixStr}]";
            }
            else
            {
                tasbSuffixStr = $"`{nea._tableName}{tasbSuffixStr}`";
            }
            return sql.Replace(nea.TableName, tasbSuffixStr);
        }

        internal static string ConvertSuffixTableName(string tableName, string tableSuffix, string dbtype)
        {
            string tasbSuffixStr = string.Empty;
            if (tableSuffix.Contains("["))
            {
                if (tableSuffix.Contains("[DAY]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[DAY]", DateTime.Now.ToString("yyyyMMdd"));
                }
                else if (tableSuffix.Contains("[MONTH]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[MONTH]", DateTime.Now.ToString("yyyyMM"));
                }
                else if (tableSuffix.Contains("[YEAR]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[YEAR]", DateTime.Now.ToString("yyyy"));
                }
                else if (tableSuffix.Contains("[HOUR]"))
                {
                    tasbSuffixStr = tableSuffix.Replace("[HOUR]", DateTime.Now.ToString("yyyyMMddHH"));
                }
                else if (tableSuffix.Contains("[WEEK]"))
                {
                    DateTime dt = DateTime.Today;
                    int count = dt.DayOfWeek - DayOfWeek.Monday;
                    if (count == -1) count = 6;
                    string res = dt.AddDays(-count).ToString("yyyyMMdd");

                    tasbSuffixStr = tableSuffix.Replace("[WEEK]", res);
                }
                else if (tableSuffix.Contains("[WEEK7]"))
                {
                    string res = "";
                    DateTime dt = DateTime.Today;
                    if (dt.DayOfWeek == DayOfWeek.Sunday)
                        res = dt.ToString("yyyyMMdd");
                    else
                        res = dt.AddDays(7 - (dt.DayOfWeek - DayOfWeek.Sunday)).ToString("yyyyMMdd");
                    tasbSuffixStr = tableSuffix.Replace("[WEEK7]", res);
                }
                else
                {
                    tasbSuffixStr = tableSuffix;
                }
            }
            else
            {
                tasbSuffixStr = tableSuffix;
            }
            if (dbtype == "MsSql")
            {
                tasbSuffixStr = $"{tableName}{tasbSuffixStr}]";
            }
            else
            {
                tasbSuffixStr = $"`{tableName}{tasbSuffixStr}`";
            }
            return tasbSuffixStr;
        }
    }
}
