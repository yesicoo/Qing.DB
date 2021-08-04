using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.DBHelper
{
    public interface IDBHelper
    {
        List<string> GetDBTables(string sqlConnStr);
        List<ColumnItem> GetTableColumns(string sqlConnStr, string tableName);
        Dictionary<string,List<ColumnItem>> GetDBColumns(string sqlConnStr);
    }
}
