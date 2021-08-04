using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Model
{
    public class ParamSql
    {
        public string SqlStr { get; set; }
        public DynamicParameters Params { get; set; } =new DynamicParameters();
        public ParamSql(string sql)
        {
            this.SqlStr = sql;
        }
        public ParamSql()
        {
        }
       
    }
}
