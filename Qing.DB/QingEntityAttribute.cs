using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QingEntityAttribute : Attribute
    {
        internal string _tableName;
        internal string _dbType;
        internal string _Lr = "`";
        internal string _Rr = "`";
        public string TableName
        {
            set { _tableName = value; }
            get
            {
                if (_dbType == "MsSql")
                {
                    return  $"[dbo].[{_tableName}]";
                }
                else
                {
                   return  $"`{_tableName}`";
                }
            }
        }
        public string PrimaryKey { set; get; }

        public string Auto_Increment { set; get; }

        internal  string ToSqlDBName(string dbid)
        {
            if (_dbType == "MsSql")
            {
                return $"[{dbid}]";
            }
            else
            {
                return $"`{dbid}`";
            }
        }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class QingIndexAttribute : Attribute
    {
        public string IndexName { set; get; }
        public string IndexFileds { set; get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class QingPropertyAttribute : Attribute
    {
        public string Alias { set; get; }
        public string DBType { set; get; }
        public string Length { set; get; }
        public bool PrimaryKey { set; get; } = false;
        public bool Auto_Increment { set; get; } = false;
    }
}
