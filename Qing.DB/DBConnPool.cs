using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using Qing.DB.Model;
using System.Collections.Concurrent;

namespace Qing.DB
{
    public static class DBConnPools
    {
        static ConcurrentDictionary<string, List<DBConnectionItem>> Dic_DBConnections = new ConcurrentDictionary<string, List<DBConnectionItem>>();
       // static List<DBConnectionItem> DBConnections = new List<DBConnectionItem>();
        static ConcurrentDictionary<string, DBConnInfo> _DBConnItems = new ConcurrentDictionary<string, DBConnInfo>();
        static string ConnStr = string.Empty;
        public static int MaxConnCount = 580;
        public static int TimeoutCheckMS = 500;
        /// <summary>
        /// 连接空闲时间（之后被清理）
        /// </summary>
        public static int ClearSpanMin = 10;
        public static bool _enable = true;
        public static void SetConnStr(string tag, string connStr, string dbType)
        {
            DBConnInfo connInfo = new DBConnInfo();
            connInfo.DBConnStr = connStr;
            connInfo.DBType = dbType;
            connInfo.Tag = tag;
            if (dbType == "MySql")
            {
                connInfo.DefaultDBName = connStr.Split(';').FirstOrDefault(x => x.ToLower().Contains("database"))?.Split('=')?[1]?.Trim();
            }
            else
            {
                connInfo.DefaultDBName = connStr.Split(';').FirstOrDefault(x => x.ToLower().Contains("initial catalog"))?.Split('=')?[1]?.Trim();
            }
            SqlLogUtil.SendLog(LogType.Msg, tag + " DefaultDB:" + connInfo.DefaultDBName);
            _DBConnItems.AddOrUpdate(tag, connInfo, (k, ov) => connInfo);
        }
        internal static string DefaultDBID(string tag = null)
        {

            var dbConnInfo = tag == null ? _DBConnItems.First().Value : _DBConnItems.TryGetValue(tag);
            return dbConnInfo?.DefaultDBName;
        }
        internal static string DBType(string tag = null)
        {
            var dbConnInfo = tag == null ? _DBConnItems.First().Value : _DBConnItems.TryGetValue(tag);
            return dbConnInfo?.DBType;
        }
        /// <summary>
        /// 当前数据库连接数量
        /// </summary>
        public static int ConnCount { get { return Dic_DBConnections.Values.Sum(x => x.Count); } }
        public static int BusyCount { get { return Dic_DBConnections.Values.Sum(x => x.Count(y => !y.Available)); } }
        public static int FreeCount { get { return Dic_DBConnections.Values.Sum(z => z.Count(x => x.Available)); } }

        #region 依据数据库名称获取门店数据库连接对象
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        //public static DBConnectionItem GetConnectionByDBID(string dbid, string tag = null)
        //{
        //    DBConnectionItem result = null;
        //    DBConnectionItem conn = null;

        //    if (!_enable)
        //    {
        //        SqlLogUtil.SendLog(LogType.Msg, "连接池已停止使用");
        //        return result;
        //    }

        //    try
        //    {
        //         conn = _GetAvailableConnection();
        //        if (conn != null)
        //        {
        //            var checkResult = Tools.TimeoutCheck(TimeoutCheckMS, () =>
        //            {
        //                if (conn.SqlConnection.State != ConnectionState.Open || !conn.PingTest())
        //                    return null;
        //                else
        //                    return conn;
        //            });
        //            //检测失败的丢弃掉重建
        //            if (checkResult == null)
        //            {
        //                SqlLogUtil.SendLog(LogType.Msg, "连接池获取的链接检测超时，释放重建");

        //                result = new DBConnectionItem();
        //                result.SqlConnection = DBFactory.Instance.CreateSqlConnection(ConnStr);// new MySqlConnection(ConnStr);
        //                result.SqlConnection.Open();
        //                result.Available = false;
        //                SqlLogUtil.SendLog(LogType.Msg, "新连接创建完成");

        //                Task.Run(() =>
        //                {
        //                    _DelAndAddConnection(conn, result);
        //                });
        //            }
        //            else
        //            {
        //                conn.Available = false;
        //                result = conn;
        //            }
        //        }
        //        else
        //        {
        //            result = new DBConnectionItem();
        //            result.SqlConnection = DBFactory.Instance.CreateSqlConnection(ConnStr);//new MySqlConnection(ConnStr);
        //            result.SqlConnection.Open();
        //            result.Available = false;
        //            _AddConnection(result);
        //        }

        //        if (result != null)
        //        {
        //            if (!string.IsNullOrEmpty(dbid) && result.SqlConnection.Database != dbid)
        //            {
        //                result.SqlConnection.ChangeDatabase(dbid);
        //            }
        //            result.Available = false;
        //        }
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        SqlLogUtil.SendLog(LogType.Error, ex.ToString());
        //        if (result != null)
        //        {
        //            result.Available = false;
        //            result.SqlConnection.Dispose();
        //            _RemoveConnection(result);
        //        }
        //        if (conn != null)
        //        {
        //            try
        //            {
        //                conn.SqlConnection.Dispose();
        //            }
        //            catch
        //            {
        //            }
        //            finally
        //            {
        //                _RemoveConnection(conn);
        //            }
        //        }
        //        return null;
        //    }
        //}
        public static DBConnectionItem GetConnectionByDBID(string dbid, string tag = null)
        {
            DBConnectionItem result = null;
            DBConnectionItem conn = null;
            if (!_enable)
            {
                SqlLogUtil.SendLog(LogType.Msg, "连接池已停止使用");
                return result;
            }
            try
            {
                conn = _GetAvailableConnection(tag);
                if (conn != null)
                {

                    //检测失败的丢弃掉重建
                    if (!conn.DBConnectionCheck())
                    {
                        try
                        {
                            SqlLogUtil.SendLog(LogType.Msg, "连接池获取的链接检测不可用，释放重建");
                            var dbConnItem = tag == null ? _DBConnItems.First().Value : _DBConnItems.TryGetValue(tag);
                            result = new DBConnectionItem(dbConnItem);
                            result.Tag = tag;
                            result.ConnectionOpen();
                            result.Available = false;
                            SqlLogUtil.SendLog(LogType.Msg, "新连接创建完成");

                        }
                        catch (Exception ex)
                        {
                            SqlLogUtil.SendLog(LogType.Error, "创建新的连接失败:" + ex.Message);
                        }
                        finally
                        {
                            Task.Run(() =>
                            {
                                _DelAndAddConnection(conn, result);
                            });
                        }
                    }
                    else
                    {
                        conn.Available = false;
                        result = conn;
                    }
                }
                else
                {
                    var dbConnItem = tag == null ? _DBConnItems.First().Value : _DBConnItems.TryGetValue(tag);
                    result = new DBConnectionItem(dbConnItem);
                    result.Tag = tag;
                    result.ConnectionOpen();
                    result.Available = false;
                    _AddConnection(result, tag);
                }

                if (result != null)
                {
                    if (string.IsNullOrEmpty(dbid))
                    {
                        dbid = result.ConnInfo.DefaultDBName;
                    }
                    if (result.SqlConnection.Database != dbid)
                    {
                        result.SqlConnection.ChangeDatabase(dbid);
                    }
                    result.Available = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Error, ex.ToString());
                if (result != null)
                {
                    result.Available = false;
                    try
                    {
                        result.SqlConnection.Dispose();
                    }
                    finally
                    {
                        _RemoveConnection(result, tag);
                    }
                }
                if (conn != null)
                {
                    try
                    {
                        conn.SqlConnection.Dispose();
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _RemoveConnection(conn, tag);
                    }
                }

                return null;
            }
        }


        #endregion




        private static DBConnectionItem _GetAvailableConnection(string tag = null)
        {
            if (tag == null) tag = "Default";
            var DBConnections =  Dic_DBConnections.TryGetValue(tag);
            if (DBConnections == null)
            {
                DBConnections = new List<DBConnectionItem>();
                Dic_DBConnections.TryAdd(tag, DBConnections);
                return null;
            }
            else
            {
                lock (DBConnections)
                {

                    var conn = DBConnections.OrderByDescending(x => x.SleepTime).FirstOrDefault(x => x.Available);
                    if (conn != null)
                    {
                        conn.Available = false;
                    }
                    return conn;
                }
            }
        }

        private static void _RemoveConnection(DBConnectionItem conn, string tag = null)
        {
            var DBConnections = tag == null ? Dic_DBConnections.First().Value : Dic_DBConnections.TryGetValue(tag);
            lock (DBConnections)
            {
                DBConnections.Remove(conn);
            }
        }

        private static void _AddConnection(DBConnectionItem conn, string tag = null)
        {
            var DBConnections = tag == null ? Dic_DBConnections.First().Value : Dic_DBConnections.TryGetValue(tag);
            lock (DBConnections)
            {
                DBConnections.Add(conn);
            }
        }
        private static void _DelAndAddConnection(DBConnectionItem oldconn, DBConnectionItem newconn, string tag = null)
        {
            var DBConnections = tag == null ? Dic_DBConnections.First().Value : Dic_DBConnections.TryGetValue(tag);
            lock (DBConnections)
            {
                oldconn.Available = false;
                DBConnections.Remove(oldconn);
                DBConnections.Add(newconn);
            }
            oldconn.SqlConnection.Disposed += (e, s) => SqlLogUtil.SendLog(LogType.Msg, "链接释放完成");
            oldconn.SqlConnection.Dispose();

        }


        #region 释放连接
        /// <summary>
        /// 释放连接
        /// </summary>
        public static void DisposeUnAvailableConn()
        {
            var expTime = DateTime.Now.AddMinutes(-ClearSpanMin);
            foreach (var item in Dic_DBConnections)
            {
                var DBConnections = item.Value;

                if (DBConnections.Count > 0)
                {
                    var freeConns = DBConnections.Where(x => x.SleepTime < expTime && x.Available).ToList();
                    foreach (var conn in freeConns)
                    {
                        conn.Available = false;
                        conn.SqlConnection.Close();
                        conn.SqlConnection.Dispose();
                    }

                    for (int i = 0; i < freeConns.Count; i++)
                    {
                        DBConnections.Remove(freeConns[i]);
                    }
                }
            }

        }

        public static void Dispose()
        {
            _enable = false;
            foreach (var item in Dic_DBConnections)
            {
                var DBConnections = item.Value;
                lock (DBConnections)
                {
                    SqlLogUtil.SendLog(LogType.Msg, item.Key + " 释放全部连接->释放前:" + DBConnections.Count);
                    var DisposeResult = Tools.TimeoutCheck(30000, () =>
                    {
                        var endTime = DateTime.Now.AddSeconds(40);
                        while (DBConnections.Count > 0 && DateTime.Now < endTime)
                        {
                            var freeConns = DBConnections.Where(x => x.Available).ToList();
                            foreach (var conn in freeConns)
                            {
                                conn.Available = false;
                                conn.SqlConnection.Close();
                                conn.SqlConnection.Dispose();
                            }

                            for (int i = 0; i < freeConns.Count; i++)
                            {
                                DBConnections.Remove(freeConns[i]);
                            }
                        }
                        return true;
                    });
                    SqlLogUtil.SendLog(LogType.Msg, item.Key + " 释放全部连接->释放后:" + DBConnections.Count);
                }
            }
        }
        #endregion
    }
}
