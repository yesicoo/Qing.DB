using Qing.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    public static class TableWatcher
    {
        public static void CreateTable<T>(string dbid = null, string tableSuffix = null, bool IsForce = false, string tag = null) where T : BaseEntity
        {
            CreateTable(typeof(T), dbid, tableSuffix, IsForce, tag);
        }

        internal static void CreateTable(Type type, string dbid = null, string tableSuffix = null, bool IsForce = false, string tag = null)
        {
            dbid = dbid ?? DBFactory.Instance.DefaultDBID();
            var nea = Tools.GetNEA(type, tag);
            if (nea != null)
            {
                var indexs = (QingIndexAttribute[])Attribute.GetCustomAttributes(type, typeof(QingIndexAttribute));
                var primaryKeys = string.IsNullOrEmpty(nea.PrimaryKey) ? new List<string>() : nea.PrimaryKey.Split(',').ToList();
                var Auto_Increments = string.IsNullOrEmpty(nea.Auto_Increment) ? new List<string>() : nea.Auto_Increment.Split(',').ToList();
                var t_Properties = type.GetProperties();
                StringBuilder sb_index = new StringBuilder();
                StringBuilder sb_Sql = new StringBuilder();
                if (nea._dbType == "MsSql")
                {
                    foreach (var propertie in t_Properties)
                    {
                        var np = (QingPropertyAttribute)Attribute.GetCustomAttribute(propertie, typeof(QingPropertyAttribute));

                        StringBuilder sb = new StringBuilder();
                        bool isPK = false;
                        sb.Append($"{nea._Lr}{propertie.Name}{nea._Rr}");
                        string sqlType = string.Empty;
                        if (np != null && !string.IsNullOrEmpty(np.DBType))
                        {
                            sb.Append($" {np.DBType}({np.Length})");
                        }
                        else
                        {
                            if (propertie.PropertyType == typeof(string))
                            {
                                sb.Append($" varchar({np?.Length ?? "255"})");
                            }
                            else if (propertie.PropertyType == typeof(DateTime))
                            {
                                sb.Append(" datetime");
                            }
                            else if (propertie.PropertyType == typeof(decimal))
                            {
                                sb.Append($" decimal({np?.Length ?? "15, 3"})");
                            }
                            else if (propertie.PropertyType == typeof(int))
                            {
                                sb.Append($" int");
                            }
                            else
                            {
                                sb.Append($" varchar({np?.Length ?? "255"})");
                            }
                        }
                        if (Auto_Increments.Contains(propertie.Name) || (np != null && np.Auto_Increment))
                        {
                            sb.Append($" identity(1,1)");
                        }
                        if (primaryKeys.Contains(propertie.Name) || (np != null && np.PrimaryKey))
                        {
                            sb.Append($" primary key NOT NULL");
                            isPK = true;
                        }

                        sb.Append(",");
                        if (isPK)
                        {
                            sb_Sql.Insert(0, sb);
                        }
                        else
                        {
                            sb_Sql.Append(sb);
                        }
                    }

                    if (indexs.Length > 0)
                    {
                        //
                        foreach (var item in indexs)
                        {
                            string indexFileds = string.Join($"{nea._Rr},{nea._Lr}", item.IndexFileds.Split(','));
                            sb_index.Append($"CREATE NONCLUSTERED INDEX {item.IndexName} ON {nea.TableSuffixName(tableSuffix)} ({indexFileds});");
                        }
                    }
                    string createTableSql = $"CREATE TABLE {nea.TableSuffixName(tableSuffix)}  ({sb_Sql.ToString().TrimEnd(',')});{sb_index}";
                    if (IsForce)
                    {
                        createTableSql = $"DROP TABLE IF EXISTS  {nea.TableSuffixName(tableSuffix)};" + createTableSql;
                    }
                    else
                    {
                        createTableSql = $"if not exists (select * from sysobjects where id = object_id('{nea.TableSuffixName(tableSuffix)}') and OBJECTPROPERTY(id, 'IsUserTable') = 1) " + createTableSql;
                    }

                   
                    int result = DBFactory.Instance.Execute(dbid, createTableSql);
                    SqlLogUtil.SendLog(LogType.Msg, "Create Table:" + nea.TableSuffixName(tableSuffix) + " -->" + result);
                }
                else
                {
                    foreach (var propertie in t_Properties)
                    {
                        var np = (QingPropertyAttribute)Attribute.GetCustomAttribute(propertie, typeof(QingPropertyAttribute));
                        string sqlType = string.Empty;
                        if (np != null && !string.IsNullOrEmpty(np.DBType))
                        {
                            sqlType = $"{np.DBType}({np.Length})";
                        }
                        else
                        {
                            if (propertie.PropertyType == typeof(string))
                            {
                                sqlType = $"varchar({np?.Length ?? "255"})";
                            }
                            else if (propertie.PropertyType == typeof(DateTime))
                            {
                                sqlType = "datetime";
                            }
                            else if (propertie.PropertyType == typeof(decimal))
                            {
                                sqlType = $"decimal({np?.Length ?? "15, 3"})";
                            }
                            else if (propertie.PropertyType == typeof(int))
                            {
                                sqlType = $"int({np?.Length ?? "10"})";
                            }
                            else
                            {
                                sqlType = $"varchar({np?.Length ?? "255"})";
                            }
                        }
                        if (np != null && np.PrimaryKey)
                        {
                            primaryKeys.Add(propertie.Name);
                        }

                        if (np != null && np.Auto_Increment)
                        {
                            Auto_Increments.Add(propertie.Name);
                        }

                        sb_Sql.Append($"{nea._Lr}{propertie.Name}{nea._Rr} {sqlType} {(primaryKeys.Contains(propertie.Name) ? "NOT NULL" : "")} {(Auto_Increments.Contains(propertie.Name) ? "AUTO_INCREMENT" : "")} COMMENT '{np?.Alias ?? ""}',");

                    }
                    if (primaryKeys.Count > 0)
                    {
                        sb_Sql.Append($"PRIMARY KEY ({nea._Lr}{string.Join($"{nea._Rr},{nea._Lr}", primaryKeys)}{nea._Rr})");
                    }
                    if (indexs.Length > 0)
                    {
                        foreach (var item in indexs)
                        {
                            string indexFileds = string.Join($"{nea._Rr},{nea._Lr}", item.IndexFileds.Split(','));
                            sb_Sql.Append($",KEY {nea._Lr}{item.IndexName}{nea._Rr} ({nea._Lr}{indexFileds}{nea._Rr})");
                        }
                    }
                    if (IsForce)
                    {
                        string createTableSql = $"DROP TABLE IF EXISTS {nea._Lr}{dbid}{nea._Rr}.{nea.TableSuffixName(tableSuffix)};CREATE TABLE {nea._Lr}{dbid}{nea._Rr}.{nea.TableSuffixName(tableSuffix)}  ({sb_Sql.ToString().TrimEnd(',')});";
                        SqlLogUtil.SendLog(LogType.Msg, "Create Table:" + nea.TableSuffixName(tableSuffix));
                       
                        DBFactory.Instance.Execute(dbid, createTableSql);
                    }
                    else
                    {
                        string checkSql = $"SELECT COUNT(*) FROM information_schema.TABLES WHERE table_name = '{nea.TableSuffixName(tableSuffix)}' AND TABLE_SCHEMA='{dbid}'";
                        var tab = DBFactory.Instance.QueryFirstOrDefault<int>(dbid, checkSql);
                        if (tab == 0)
                        {
                            string createTableSql = $"CREATE TABLE {nea._Lr}{dbid}{nea._Rr}.{nea.TableSuffixName(tableSuffix)}  ({sb_Sql.ToString().TrimEnd(',')});";
                            SqlLogUtil.SendLog(LogType.Msg, "Create Table:" + nea.TableSuffixName(tableSuffix));
                           
                            DBFactory.Instance.Execute(dbid, createTableSql);
                        }
                    }
                }
            }
        }

    }
}
