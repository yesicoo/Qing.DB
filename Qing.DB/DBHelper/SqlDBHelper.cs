using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Qing.DB.DBHelper
{
    public class SqlDBHelper : IDBHelper
    {
        public  Dictionary<string, List<ColumnItem>> GetDBColumns(string sqlConnStr)
        {
            Dictionary<string, List<ColumnItem>> DBColumns = new Dictionary<string, List<ColumnItem>> ();
            //if (nea._dbType == "MsSql")
            //{
            //    using (SqlConnection conn = new SqlConnection(sqlConnStr))
            //    {
            //        var tables = conn.Query<string>("SHOW TABLES");
            //        foreach (var item in tables)
            //        {
            //            var columns = conn.Query<ColumnItem>("show FULL columns from " + item).ToList();
            //            DBColumns.Add(item, columns);
            //        }
            //    }
            //}
            //else { 
            //    using (MySqlConnection conn = new MySqlConnection(sqlConnStr))
            //    {
            //        var tables = conn.Query<string>("SHOW TABLES");
            //        foreach (var item in tables)
            //        {
            //            var columns = conn.Query<ColumnItem>("show FULL columns from " + item).ToList();
            //            DBColumns.Add(item, columns);
            //        }
            //    }
            //}
            return DBColumns;
        }

        public List<string> GetDBTables(string sqlConnStr)
        {
            List<string> tables = new List<string>();
            //if (nea._dbType == "MsSql")
            //{
            //    //using (SqlConnection conn = new SqlConnection(sqlConnStr))
            //    //{
            //    //    tables = conn.Query<string>("SHOW TABLES").ToList();
            //    //}
            //}
            //else
            //{
            //    using (MySqlConnection conn = new MySqlConnection(sqlConnStr))
            //    {
            //        tables = conn.Query<string>("SHOW TABLES").ToList();
            //    }
            //}
            return tables;
        }

        public List<ColumnItem> GetTableColumns(string sqlConnStr, string tableName)
        {
            List<ColumnItem> columns = null;
            //if (nea._dbType == "MsSql")
            //{
            //    //using (SqlConnection conn = new SqlConnection(sqlConnStr))
            //    //{
            //    //    columns = conn.Query<ColumnItem>("show FULL columns from " + tableName).ToList();
            //    //}
            //}
            //else
            //{
            //    using (DbConnection conn = new MySqlConnection(sqlConnStr))
            //    {
            //        columns = conn.Query<ColumnItem>("show FULL columns from " + tableName).ToList();
            //    }
            //}
            return columns;
        }
    }
}
