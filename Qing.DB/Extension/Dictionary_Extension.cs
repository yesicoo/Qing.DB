using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    internal static class Dictionary_Extension
    {
        #region TryGetValue
        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static object TryGetValue(this Dictionary<string, object> dic, string key)
        {
            object result = null;
            dic.TryGetValue(key, out result);
            return result;
        }
        internal static T TryGetValue<T>(this ConcurrentDictionary<string, T> dic, string key)
        {

            dic.TryGetValue(key, out var result);
            return result;
        }
        #endregion

        internal static void AddOrUpdate(this Dictionary<string, object> dic, string key, object value)
        {
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key] = value;
                }
                else
                {
                    dic.Add(key, value);
                }
            }
        }

        internal static void Add(this Dictionary<string, object> dic, string key, object value,Type dataType)
        {
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    dic[key] = Convert.ChangeType(value,dataType);
                }
                else
                {
                    dic.Add(key, Convert.ChangeType(value, dataType));
                }
            }
        }

        internal static void AddRange(this Dictionary<string, object> dic, Dictionary<string,object> newDic)
        {
            lock (dic)
            {
                foreach (var item in newDic)
                {
                    if (dic.ContainsKey(item.Key))
                    {
                        dic[item.Key] = item.Value;
                    }
                    else
                    {
                        dic.Add(item.Key, item.Value);
                    }
                }
            }
        }


        internal static bool TryRemove(this Dictionary<string, object> dic, string key)
        {
            lock (dic)
            {
                if (dic.ContainsKey(key))
                {
                    return dic.Remove(key);
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
