using Dapper;
using Qing.DB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB
{
    public static class Entity_Extension
    {
        #region 生成SQL语句

        #region 查询
        /// <summary>
        /// 获取查询所有的sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>


        internal static ParamSql GetAll_SQL<T>(this T t, string DBID) where T : BaseEntity
        {
            var nea = t.GetNEA();
            return new ParamSql($" select {t.GetBaseFieldsStr()} from {nea.ToSqlDBName(DBID)}.{nea.TableName} ");
        }

        #region GetAll_SQL


        /// <summary>
        /// 查找所有并排序返回
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="orderBy"></param>
        /// <param name="IsDesc"></param>
        /// <returns></returns>
        internal static ParamSql GetAll_SQL<T>(this T t, string DBID, string orderBy, bool IsDesc = false) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            var nea = t.GetNEA();
            StringBuilder sb_ob = new StringBuilder($" select {t.GetBaseFieldsStr()} from {nea.ToSqlDBName(DBID)}.{nea.TableName} ");
            if (!string.IsNullOrEmpty(orderBy))
            {
                sb_ob.Append($" Order By @OrderBy");
                ps.Params.Add("OrderBy", orderBy);
                if (IsDesc)
                {
                    sb_ob.Append(" Desc ");
                }
            }
            ps.SqlStr = sb_ob.ToString();
            return ps;
        }
        #endregion

        /// <summary>
        /// 根据主键查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="PrimaryValue"></param>
        /// <returns></returns>
        internal static ParamSql GetByPrimaryKey_SQL<T>(this T t, string DBID, params object[] PrimaryValue) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            var nea = t.GetNEA();

            var pkvs = nea.PrimaryKey.Split(',').ToList();
            if (pkvs != null && pkvs.Count > 0)
            {
                string whereStr = string.Empty;
                if (pkvs.Count != PrimaryValue.Length)
                {
                    SqlLogUtil.SendLog(LogType.Error, "主键数量不匹配");
                    return null;

                }
                else
                {
                    for (int i = 0; i < pkvs.Count; i++)
                    {
                        whereStr += $" and {pkvs[i]} = @{pkvs[i]}";
                        ps.Params.Add(pkvs[i], PrimaryValue[i]);
                    }
                    if (!string.IsNullOrEmpty(whereStr))
                    {
                        ps.SqlStr = $"select * from {nea.ToSqlDBName(DBID)}.{nea.TableName} where 1=1 {whereStr};";
                        return ps;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }






        /// <summary>
        /// 获取主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetPrimaryKey<T>(this T t) where T : BaseEntity
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            var nea = t.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = t.GetType().GetProperties();
            var pks = pros.Where(x => primaryKeys.Contains(x.Name));
            if (pks != null)
            {
                foreach (var pk in pks)
                {
                    result.Add(pk.Name, pk.GetValue(t).ToString());
                }
            }
            return result;

        }
        internal static Dictionary<string, List<string>> GetPrimaryKey<T>(this List<T> ents) where T : BaseEntity
        {
            Dictionary<string, List<string>> dic_PrimaryKeys = new Dictionary<string, List<string>>();
            var firstEnt = ents.First();
            var nea = firstEnt.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = firstEnt.GetType().GetProperties();
            var pks = pros.Where(x => primaryKeys.Contains(x.Name));
            if (pks != null)
            {
                foreach (var pk in pks)
                {
                    dic_PrimaryKeys.Add(pk.Name, new List<string>());

                    foreach (var ent in ents)
                    {
                        dic_PrimaryKeys[pk.Name].Add(pk.GetValue(ent).ToString());
                    }
                }
            }
            return dic_PrimaryKeys;
        }

        #endregion

        #region 删除
        internal static ParamSql GetDelByPrimaryKey_SQL<T>(this T t, string DBID) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (t == null) { return null; }
            var nea = t.GetNEA();
            var pkvs = GetPrimaryKey(t);
            if (pkvs != null && pkvs.Keys.Count > 0)
            {
                string whereStr = string.Empty;
                foreach (var pkv in pkvs)
                {
                    whereStr += $"and {pkv.Key} = @{pkv.Key} ";
                    ps.Params.Add(pkv.Key, pkv.Value);
                }
                if (!string.IsNullOrEmpty(whereStr))
                {
                    ps.SqlStr = $"delete from {nea.ToSqlDBName(DBID)}.{nea.TableName} where 1=1 {whereStr};";
                    return ps;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        internal static string GetDelByPrimaryKey_SQL<T>(this List<T> ents, string DBID) where T : BaseEntity
        {
            var nea = ents.First().GetNEA();
            var pkvs = GetPrimaryKey(ents);
            if (pkvs != null && pkvs.Keys.Count > 0)
            {
                string whereStr = string.Empty;
                foreach (var pkv in pkvs)
                {
                    whereStr += $"and {pkv.Key} in ('{string.Join("','", pkv.Value.Distinct().ToArray())}') ";
                }
                if (!string.IsNullOrEmpty(whereStr))
                {
                    String sql = $"delete from {nea.ToSqlDBName(DBID)}.{nea.TableName} where 1=1 {whereStr};";
                    return sql;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region 插入
        /// <summary>
        ///  获取插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="dbID"></param>
        /// <param name="ignoreInc">忽略自增字段</param>
        /// <returns></returns>
        internal static ParamSql GetInsert_Sql<T>(this T t, string dbID, string tableSuffix = null,bool ignoreInc = false,string tag=null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (t == null) { return null; }
            var nea = t.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            List<string> Auto_Increments = null;
            if (!string.IsNullOrEmpty(nea.Auto_Increment))
            {
                Auto_Increments = nea.Auto_Increment.Split(',').ToList();
            }
            else
            {
                Auto_Increments = new List<string>();
            }

            PropertyInfo[] pros = t.GetType().GetProperties();
            string InsertSQL = "Insert into {0} ({1}) values({2});";

            StringBuilder sb_Columns = new StringBuilder();
            StringBuilder sb_Values = new StringBuilder();
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {
                    if (!ignoreInc && Auto_Increments.Contains(item.Name))
                    {
                        continue;
                    }
                    if (item.PropertyType == typeof(DateTime))
                    {
                        DateTime? value = (DateTime)(item.GetValue(t, null));
                        if (value == DateTime.MinValue && nea._dbType == "MsSql")
                        {
                            value = null;// System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                        }

                        sb_Columns.Append($"{nea._Lr}{item.Name}{nea._Rr},");
                        sb_Values.Append($"@{item.Name},");
                        ps.Params.Add(item.Name, value, DbType.DateTime);

                    }
                    else
                    {
                        var value = item.GetValue(t, null);
                        if (value != null)
                        {
                            sb_Columns.Append($"{nea._Lr}{item.Name}{nea._Rr},");
                            sb_Values.Append($"@{item.Name},");
                            ps.Params.Add(item.Name, value);
                        }
                    }
                }
            }
            string sql= string.Format(InsertSQL, (nea.ToSqlDBName(dbID) + "." + nea.TableName), sb_Columns.ToString().TrimEnd(','), sb_Values.ToString().TrimEnd(','));
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;
        }
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="DBID"></param>
        /// <param name="ignoreInc">忽略自增ID</param>
        /// <returns></returns>
        internal static ParamSql GetInsert_Sql<T>(this List<T> ts, string DBID,string tableSuffix= null,bool ignoreInc = false, string tag = null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (ts.Count == 0)
            {
                return null;
            }
            List<string> excludeColumns = new List<string>();
            var _t = ts.FirstOrDefault();
            var nea = _t.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            var Auto_Increments = nea.Auto_Increment.Split(',').ToList();
            PropertyInfo[] pros = _t.GetType().GetProperties();
            StringBuilder sb_Columns = new StringBuilder();
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {
                    if (!ignoreInc && Auto_Increments.Contains(item.Name))
                    {
                        continue;
                    }
                    sb_Columns.Append($"{nea._Lr}{item.Name}{nea._Rr},");
                }
            }

            StringBuilder sb_SQL = new StringBuilder($"Insert into {nea.ToSqlDBName(DBID)}.{nea.TableName} ({ sb_Columns.ToString().TrimEnd(',')}) values ");


            for (int i = 0; i < ts.Count(); i++)
            {
                var t = ts[i];
                StringBuilder sb_Values = new StringBuilder("(");
                foreach (var item in pros)
                {
                    if (item.CanRead && item.CanWrite)
                    {
                        if (Auto_Increments.Contains(item.Name))
                        {
                            continue;
                        }
                        if (item.PropertyType == typeof(DateTime))
                        {
                            //var value = Convert.ToDateTime(item.GetValue(t, null));
                            //sb_Values.Append($"@{item.Name}_{i},");
                            //ps.Params.Add($"{item.Name}_{i}", value.ToString("yyyy-MM-dd HH:mm:ss"));

                            DateTime? value = (DateTime)(item.GetValue(t, null));
                            if (value == DateTime.MinValue && nea._dbType == "MsSql")
                            {
                                value = null;// System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                            }

                            sb_Columns.Append($"{nea._Lr}{item.Name}{nea._Rr},");
                            sb_Values.Append($"@{item.Name}_{i},");
                            ps.Params.Add($"{item.Name}_{i}", value, DbType.DateTime);
                        }
                        else
                        {
                            var value = item.GetValue(t, null);
                            sb_Values.Append($"@{item.Name}_{i},");
                            ps.Params.Add($"{item.Name}_{i}", value);
                        }
                    }
                }
                string values = sb_Values.ToString().TrimEnd(',') + "),";
                sb_SQL.Append(values);
            }

            string sql=sb_SQL.ToString().TrimEnd(',') + ";";
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;
        }

        #endregion

        #region 更改
        internal static ParamSql GetUpdateByPrimaryKey_Sql<T>(this T t, string DBID, string tableSuffix = null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (t == null) { return null; }
            Dictionary<string, string> Primarys = new Dictionary<string, string>();
            List<string> excludeColumns = new List<string>();
            var nea = t.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = t.GetType().GetProperties();
            StringBuilder sb_SQL = new StringBuilder($"Update {nea.ToSqlDBName(DBID)},{nea.TableName} set ");
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {
                    if (primaryKeys.Contains(item.Name))
                    {

                        var value = item.GetValue(t, null).ToString();
                        Primarys[item.Name] = value;
                        continue;
                    }

                    if (item.PropertyType == typeof(DateTime))
                    {
                        DateTime? value = (DateTime)(item.GetValue(t, null));
                        if (value == DateTime.MinValue && nea._dbType == "MsSql")
                        {
                            value = null;
                        }
                        sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                        ps.Params.Add($"{item.Name}", value, DbType.DateTime);

                    }
                    else
                    {
                        var value = item.GetValue(t, null);
                        if (value != null)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, value);
                        }
                    }
                }
            }
            StringBuilder whereStr = new StringBuilder(" where 1=1 ");
            foreach (var item in Primarys)
            {
                whereStr.Append($"and {nea._Lr}{item.Key}{nea._Rr}=@{item.Key}_PK ");
                ps.Params.Add(item.Key + "_PK", item.Value);
            }
            string sql = sb_SQL.ToString().TrimEnd(',') + whereStr.ToString() + ";";
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;

        }

        internal static ParamSql GetUpdateByPrimaryKey_Sql<T>(this List<T> ents, string DBID,string tableSuffix=null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (ents == null || ents.Count == 0) { return null; }
            Dictionary<string, string> Primarys = new Dictionary<string, string>();
            var nea = ents.First().GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = nea.GetType().GetProperties();
            StringBuilder sb_lang_sql = new StringBuilder();
            for (int i = 0; i < ents.Count; i++)
            {
                var ent = ents[i];

                StringBuilder sb_SQL = new StringBuilder($"Update {nea.ToSqlDBName(DBID)}.{nea.TableName} set ");
                foreach (var item in pros)
                {
                    if (item.CanRead && item.CanWrite)
                    {
                        if (primaryKeys.Contains(item.Name))
                        {

                            var value = item.GetValue(ent, null).ToString();
                            Primarys[item.Name] = value;
                            continue;
                        }

                        if (item.PropertyType == typeof(DateTime))
                        {
                            var value = Convert.ToDateTime(item.GetValue(ent, null));
                            if (value != DateTime.MinValue)
                            {
                                sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name}{i},");
                                ps.Params.Add(item.Name + i, value.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        else
                        {
                            var value = item.GetValue(ent, null);
                            if (value != null)
                            {
                                sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name}{i},");
                                ps.Params.Add(item.Name + i, value.ToString());
                            }
                        }
                    }
                }
                StringBuilder whereStr = new StringBuilder(" where 1=1 ");
                foreach (var item in Primarys)
                {
                    whereStr.Append($"and {nea._Lr}{item.Key}{nea._Rr}=@{item.Key}_PK ");
                    ps.Params.Add(item.Key + "_PK", item.Value);
                }
                sb_lang_sql.Append(sb_SQL.ToString().TrimEnd(',') + whereStr.ToString() + ";");
            }
            string sql = sb_lang_sql.ToString();
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;

        }

        internal static ParamSql GetUpdateFieldsByPrimaryKey_Sql<T>(this T t, string DBID, string[] fields,string tableSuffix=null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (t == null) { return null; }
            Dictionary<string, string> Primarys = new Dictionary<string, string>();
            List<string> excludeColumns = new List<string>();
            var nea = t.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = t.GetType().GetProperties();
            StringBuilder sb_SQL = new StringBuilder($"Update {nea.ToSqlDBName(DBID)}.{nea.TableName} set ");
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {

                    if (primaryKeys.Contains(item.Name))
                    {
                        var value = item.GetValue(t, null).ToString();
                        Primarys[item.Name] = value;
                        continue;
                    }

                    if (fields != null && fields.Length > 0 && !fields.Contains(item.Name))
                    {
                        continue;
                    }

                    if (item.PropertyType == typeof(DateTime))
                    {
                        DateTime? value = (DateTime)(item.GetValue(t, null));
                        if (value == DateTime.MinValue && nea._dbType == "MsSql")
                        {
                            value = null;
                        }
                        sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                        ps.Params.Add($"{item.Name}", value, DbType.DateTime);


                    }
                    else
                    {
                        var value = item.GetValue(t, null);
                        if (value != null)
                        {

                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, value.ToString());
                        }
                    }
                }
            }
            StringBuilder whereStr = new StringBuilder(" where 1=1 ");
            foreach (var item in Primarys)
            {
                whereStr.Append($"and {nea._Lr}{item.Key}{nea._Rr}=@{item.Key}_PK ");
                ps.Params.Add(item.Key + "_PK", item.Value);
            }
            string sql = sb_SQL.ToString().TrimEnd(',') + whereStr.ToString() + ";";
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;
        }

        internal static ParamSql GetUpdateFieldsByPrimaryKey_Sql<T>(this List<T> ents, string DBID, string[] fields,string tableSuffix=null) where T : BaseEntity
        {
            ParamSql ps = new ParamSql();
            if (ents == null || ents.Count == 0) { return null; }
            Dictionary<string, string> Primarys = new Dictionary<string, string>();
            var nea = ents.First().GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            PropertyInfo[] pros = nea.GetType().GetProperties();
            StringBuilder sb_lang_sql = new StringBuilder();
            for (int i = 0; i < ents.Count; i++)
            {
                var ent = ents[i];


                StringBuilder sb_SQL = new StringBuilder($"Update {nea.ToSqlDBName(DBID)}.{nea.TableName} set ");
                foreach (var item in pros)
                {
                    if (item.CanRead && item.CanWrite)
                    {
                        if (primaryKeys.Contains(item.Name))
                        {
                            var value = item.GetValue(ent, null).ToString();
                            Primarys[item.Name] = value;
                            continue;
                        }

                        if (!fields.Contains(item.Name))
                        {
                            continue;
                        }

                        if (item.PropertyType == typeof(DateTime))
                        {
                            DateTime? value = (DateTime)(item.GetValue(ent, null));
                            if (value == DateTime.MinValue && nea._dbType == "MsSql")
                            {
                                value = null;
                            }
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name}_{i},");
                            ps.Params.Add($"{item.Name}_{i}", value, DbType.DateTime);
                        }
                        else
                        {
                            var value = item.GetValue(ent, null);
                            if (value != null)
                            {

                                sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name}{i},");
                                ps.Params.Add(item.Name + i, value.ToString());
                            }
                        }
                    }
                }
                StringBuilder whereStr = new StringBuilder(" where 1=1 ");
                foreach (var item in Primarys)
                {
                    whereStr.Append($"and {nea._Lr}{item.Key}{nea._Rr}=@{item.Key}_PK ");
                    ps.Params.Add(item.Key + "_PK", item.Value);
                }
                sb_lang_sql.AppendFormat("{0},{1};", sb_SQL.ToString().TrimEnd(','), whereStr.ToString());
            }
            string sql = sb_lang_sql.ToString();
            if (!string.IsNullOrEmpty(tableSuffix))
            {
                sql = Tools.AppendTableSuffix(nea, sql, tableSuffix);
            }
            ps.SqlStr = sql;
            return ps;
        }
        #endregion

        #endregion



        #region 辅助方法
        /// <summary>
        /// 获取基础类型字段名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetBaseFieldsStr<T>(this T t) where T : BaseEntity
        {
            return string.Join(",", typeof(T).GetProperties().Where(x => x.CanWrite && x.PropertyType.IsBaseType()).Select(x => x.Name));
        }

        internal static Dictionary<string, string> GetFieldsAlias<T>(this T t) where T : BaseEntity
        {
            Dictionary<string, string> propertiy_attribute = new Dictionary<string, string>();
            var properties = typeof(T).GetProperties();
            foreach (var propertiy in properties)
            {
                var epa = (QingPropertyAttribute)Attribute.GetCustomAttribute(propertiy, typeof(QingPropertyAttribute));
                propertiy_attribute.Add(propertiy.Name, epa.Alias);
            }
            return propertiy_attribute;
        }

        private static bool IsBaseType(this Type dataType)
        {
            if (dataType == null)
                return false;

            return (dataType == typeof(int)
                    || dataType == typeof(string)
                    || dataType == typeof(double)
                    || dataType == typeof(Decimal)
                    || dataType == typeof(DateTime)
                    || dataType == typeof(long)
                    || dataType == typeof(short)
                    || dataType == typeof(float)
                    || dataType == typeof(Int16)
                    || dataType == typeof(Int32)
                    || dataType == typeof(Int64)
                    || dataType == typeof(uint)
                    || dataType == typeof(UInt16)
                    || dataType == typeof(UInt32)
                    || dataType == typeof(UInt64)
                    || dataType == typeof(sbyte)
                    || dataType == typeof(Single)
                    || dataType == typeof(String)
                   );
        }
        #endregion


        #region Insert
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        internal static int Insert<T>(this T t, string dbid = "", string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
                return -1;

            using (DBContext context = new DBContext(dbid))
            {
                return context.Create(t, tableSuffix, ignoreInc);
            }
        }

        internal static Task<int> InsertAsync<T>(this T t, string dbid = "", string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
                return Task.FromResult(-1);
            using (DBContext context = new DBContext(dbid))
            {
                return context.CreateAsync(t, tableSuffix, ignoreInc);
            }


        }



        public static int Insert<T>(this T t, DBContext context, string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
                return -1;

            return context.Create(t, tableSuffix, ignoreInc);
        }

        public static Task<int> InsertAsync<T>(this T t, DBContext context, string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (t == null)
                return Task.FromResult(-1);

            return context.CreateAsync(t, tableSuffix, ignoreInc);
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="ents"></param>
        /// <returns></returns>
        internal static int BulkInsert<T>(this List<T> ents, string dbid = "", string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (ents == null || ents.Count() == 0)
                return -1;


            using (DBContext context = new DBContext(dbid))
            {
                return context.BatchCreate(ents, tableSuffix, ignoreInc);
            }

        }

        internal static Task<int> BulkInsertAsync<T>(this List<T> ents, string dbid = "", string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (ents == null || ents.Count() == 0)
                return Task.FromResult(-1);

            using (DBContext context = new DBContext(dbid))
            {
                return context.BatchCreateAsync(ents, tableSuffix, ignoreInc);
            }

        }



        public static int BulkInsert<T>(this List<T> ents, DBContext context, string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (ents == null || ents.Count() == 0)
                return -1;
            return context.BatchCreate(ents, tableSuffix, ignoreInc);

        }

        public static Task<int> BulkInsertAsync<T>(this List<T> ents, DBContext context, string tableSuffix = null,bool ignoreInc = false) where T : BaseEntity
        {
            if (ents == null || ents.Count() == 0)
                return Task.FromResult(-1);
            return context.BatchCreateAsync(ents, tableSuffix, ignoreInc);
        }
        #endregion

        #region Del
        /// <summary>
        /// 主键删除
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static int RemoveByPrimaryKeys<T>(this T ent, string dbid = "") where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");
            try
            {
                dbid = Tools.GetDBID(dbid);
                var ps = ent.GetDelByPrimaryKey_SQL(dbid);
                return DBFactory.Instance.Execute(dbid, ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysAsync<T>(this T ent, string dbid = "") where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");
            try
            {
                dbid = Tools.GetDBID(dbid);
                var ps = ent.GetDelByPrimaryKey_SQL(dbid);
                return DBFactory.Instance.ExecuteAsync(dbid, ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 基于事物的主键删除
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="primayValue"></param>
        /// <returns></returns>
        public static int RemoveByPrimaryKeys<T>(this T ent, IDbTransaction transaction) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetDelByPrimaryKey_SQL(transaction.Connection.Database);
                return transaction.Connection.Execute(ps.SqlStr, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysAsync<T>(this T ent, IDbTransaction transaction) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetDelByPrimaryKey_SQL(transaction.Connection.Database);
                return transaction.Connection.ExecuteAsync(ps.SqlStr, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static int RemoveByPrimaryKeys<T>(this T ent, DBContext context) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetDelByPrimaryKey_SQL(context.CurrentDB);
                return context.Execute(ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysAsync<T>(this T ent, DBContext context) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetDelByPrimaryKey_SQL(context.CurrentDB);
                return context.ExecuteAsync(ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }



        public static int RemoveByPrimaryKeys<T>(this List<T> ents, string dbid = "") where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                dbid = Tools.GetDBID(dbid);
                var sql = ents.GetDelByPrimaryKey_SQL(dbid);
                return DBFactory.Instance.Execute(dbid, sql);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysTask<T>(this List<T> ents, string dbid = "") where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                dbid = Tools.GetDBID(dbid);
                var sql = ents.GetDelByPrimaryKey_SQL(dbid);
                return DBFactory.Instance.ExecuteAsync(dbid, sql);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static int RemoveByPrimaryKeys<T>(this List<T> ents, IDbTransaction transaction) where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                var sql = ents.GetDelByPrimaryKey_SQL(transaction.Connection.Database);
                return transaction.Connection.Execute(sql, null, transaction);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysAsync<T>(this List<T> ents, IDbTransaction transaction) where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                var sql = ents.GetDelByPrimaryKey_SQL(transaction.Connection.Database);
                return transaction.Connection.ExecuteAsync(sql, null, transaction);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static int RemoveByPrimaryKeys<T>(this List<T> ents, DBContext context) where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                var sql = ents.GetDelByPrimaryKey_SQL(context.CurrentDB);
                return context.Execute(sql, null);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> RemoveByPrimaryKeysAsync<T>(this List<T> ents, DBContext context) where T : BaseEntity
        {
            if (ents == null)
                throw new Exception("实列对象不可为空");
            if (ents.Count == 0)
                throw new Exception("实列对象集合无数据");
            try
            {
                var sql = ents.GetDelByPrimaryKey_SQL(context.CurrentDB);
                return context.ExecuteAsync(sql, null);
            }
            catch (Exception)
            {

                throw;
            }

        }


        #endregion

        #region Update



        //public static int UpdateByPrimaryKeys<T>(this T ent, string dbid, params string[] fields) where T : BaseEntity
        //{
        //    dbid = Tools.GetDBID(dbid);
        //    var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(dbid, fields);
        //    return DBFactory.Instance.Execute(dbid, ps.SqlStr, ps.Params);
        //}
        public static int UpdateByPrimaryKeys<T>(this T ent, params string[] fields) where T : BaseEntity
        {
            var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(DBFactory.Instance.DefaultDBID(), fields);
            return DBFactory.Instance.Execute(DBFactory.Instance.DefaultDBID(), ps.SqlStr, ps.Params);
        }

        public static Task<int> UpdateByPrimaryKeysAsync<T>(this T ent, params string[] fields) where T : BaseEntity
        {
            var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(DBFactory.Instance.DefaultDBID(), fields);
            return DBFactory.Instance.ExecuteAsync(DBFactory.Instance.DefaultDBID(), ps.SqlStr, ps.Params);
        }

        //public static int UpdateByPrimaryKeys<T>(this T ent, IDbTransaction transaction, params string[] fields) where T : BaseEntity
        //{
        //    var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(transaction.Connection.Database, fields);
        //    return transaction.Connection.Execute(ps.SqlStr, ps.Params, transaction);
        //}
        public static int UpdateByPrimaryKeys<T>(this T ent, DBContext context, params string[] fields) where T : BaseEntity
        {
            var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(context.CurrentDB, fields);
            return context.Execute(ps.SqlStr, ps.Params);
        }

        public static Task<int> UpdateByPrimaryKeysAsync<T>(this T ent, DBContext context, params string[] fields) where T : BaseEntity
        {
            var ps = ent.GetUpdateFieldsByPrimaryKey_Sql(context.CurrentDB, fields);
            return context.ExecuteAsync(ps.SqlStr, ps.Params);
        }


        /// <summary>
        /// 通过主键更新
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static int UpdateByPrimaryKeys<T>(this T ent, string dbid = "") where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                dbid = Tools.GetDBID(dbid);
                var ps = ent.GetUpdateByPrimaryKey_Sql(dbid);
                return DBFactory.Instance.Execute(dbid, ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> UpdateByPrimaryKeysAsync<T>(this T ent, string dbid = "") where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                dbid = Tools.GetDBID(dbid);
                var ps = ent.GetUpdateByPrimaryKey_Sql(dbid);
                return DBFactory.Instance.ExecuteAsync(dbid, ps.SqlStr, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static int UpdateByPrimaryKeys<T, TResult>(this T t, Expression<Func<T, TResult>> expressionNew, string dbid = "") where T : BaseEntity
        {

            if (t == null)
                throw new Exception("实列对象不可为空");

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');

            StringBuilder sb_where = new StringBuilder(" 1=1 ");
            var pks = t.GetPrimaryKey();
           
            try
            {
                dbid = Tools.GetDBID(dbid);
                var nea = t.GetNEA();
                foreach (var pk in pks)
                {
                    sb_where.Append($" and {nea._Lr}{pk.Key}{nea._Rr}='{pk.Value}'");
                }
                string sql = $"update {nea.TableName} set {setStr} where {sb_where.ToString()};";
                return DBFactory.Instance.Execute(dbid, sql, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public static Task<int> UpdateByPrimaryKeysAsync<T, TResult>(this T t, Expression<Func<T, TResult>> expressionNew, string dbid = "") where T : BaseEntity
        {

            if (t == null)
                throw new Exception("实列对象不可为空");

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');

            StringBuilder sb_where = new StringBuilder(" 1=1 ");
            var pks = t.GetPrimaryKey();
           
            try
            {
                dbid = Tools.GetDBID(dbid);
                var nea = t.GetNEA();
                foreach (var pk in pks)
                {
                    sb_where.Append($" and {nea._Lr}{pk.Key}{nea._Rr}='{pk.Value}'");
                }
                string sql = $"update {nea.TableName} set {setStr} where {sb_where.ToString()};";
                return DBFactory.Instance.ExecuteAsync(dbid, sql, ps.Params);
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 通过主键事物更新
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="ent"></param>
        /// <returns></returns>
        public static int UpdateByPrimaryKeys<T>(this T ent, IDbTransaction transaction) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetUpdateByPrimaryKey_Sql(transaction.Connection.Database);
                return transaction.Connection.Execute(ps.SqlStr, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }


        }

        public static Task<int> UpdateByPrimaryKeysAsync<T>(this T ent, IDbTransaction transaction) where T : BaseEntity
        {
            if (ent == null)
                throw new Exception("实列对象不可为空");

            try
            {
                var ps = ent.GetUpdateByPrimaryKey_Sql(transaction.Connection.Database);
                return transaction.Connection.ExecuteAsync(ps.SqlStr, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }


        }

        public static int UpdateByPrimaryKeys<T, TResult>(this T t, Expression<Func<T, TResult>> expressionNew, IDbTransaction transaction) where T : BaseEntity
        {

            if (t == null)
                throw new Exception("实列对象不可为空");

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');

            StringBuilder sb_where = new StringBuilder(" 1=1 ");
            var pks = t.GetPrimaryKey();
           
            try
            {
                var nea = t.GetNEA();
                foreach (var pk in pks)
                {
                    sb_where.Append($" and {nea._Lr}{pk.Key}{nea._Rr}='{pk.Value}'");
                }
                string sql = $"update {nea.TableName} set {setStr} where {sb_where.ToString()};";
                return transaction.Connection.Execute(sql, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Task<int> UpdateByPrimaryKeysAsync<T, TResult>(this T t, Expression<Func<T, TResult>> expressionNew, IDbTransaction transaction) where T : BaseEntity
        {

            if (t == null)
                throw new Exception("实列对象不可为空");

            ParamSql ps = new ParamSql();
            var dic_update = ExpressionResolver.ResoveUpdateExpression(expressionNew);
            StringBuilder sb_setStr = new StringBuilder();
            foreach (var item in dic_update)
            {
                sb_setStr.Append($",{item.SqlStr}");
                ps.Params.AddDynamicParams(item.Params);
            }
            string setStr = sb_setStr.ToString().TrimStart(',');

            StringBuilder sb_where = new StringBuilder(" 1=1 ");
            var pks = t.GetPrimaryKey();
            try
            {
                var nea = t.GetNEA();
                foreach (var pk in pks)
                {
                    sb_where.Append($" and {nea._Lr}{pk.Key}{nea._Rr}='{pk.Value}'");
                }
                string sql = $"update {nea.TableName} set {setStr} where {sb_where.ToString()};";
                return transaction.Connection.ExecuteAsync(sql, ps.Params, transaction);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region FilterFields
        public static dynamic FilterFields<T>(this T t, Expression<Func<T, object>> selector) where T : BaseEntity
        {
            if (t == null)
                return null;
            if (selector == null)
                return t;
            string[] filterProperties;
            if (selector.Body.NodeType == ExpressionType.MemberAccess)
            {
                filterProperties = new string[] { ((MemberExpression)selector.Body).Member.Name };
            }
            else if (selector.Body.NodeType == ExpressionType.Convert)
            {
                filterProperties = new string[] { ((MemberExpression)((UnaryExpression)selector.Body).Operand).Member.Name };
            }
            else if (selector.Body.NodeType == ExpressionType.New)
            {
                filterProperties = ((NewExpression)selector.Body).Members.Select(m => m.Name).ToArray();
            }
            else
            {
                return t;
            }

            IDictionary<string, object> data = new ExpandoObject();
            var ps = t.GetType().GetProperties();
            foreach (var item in ps)
            {
                if (filterProperties.Contains(item.Name))
                {
                    continue;
                }
                else
                {
                    data[item.Name] = item.GetValue(t);
                }
            }
            return data;
        }
        public static IEnumerable<dynamic> FilterFields<T, TResult>(this IEnumerable<T> ts, Expression<Func<T, TResult>> selector) where T : BaseEntity
        {
            if (ts == null)
                return null;
            if (selector == null)
                return ts;
            var newExpression = selector.Body as NewExpression;
            if (newExpression == null)
                return ts;
            var filterProperties = newExpression.Members.Select(m => m.Name).ToArray();
            List<IDictionary<string, object>> datas = new List<IDictionary<string, object>>();
            var ps = ts.First().GetType().GetProperties();

            foreach (var t in ts)
            {
                IDictionary<string, object> data = new ExpandoObject();
                foreach (var item in ps)
                {
                    if (filterProperties.Contains(item.Name))
                    {
                        continue;
                    }
                    else
                    {
                        data[item.Name] = item.GetValue(t);
                    }
                }
                datas.Add(data);
            }
            return datas;
        }
        #endregion

        public static QingEntityAttribute GetNEA<T>(this T t) where T : BaseEntity
        {
            return (QingEntityAttribute)Attribute.GetCustomAttribute(t.GetType(), typeof(QingEntityAttribute));
        }


        public static TResult GetValue<T, TResult>(this T t, string filed) where T : BaseEntity
        {
            try
            {
                PropertyInfo[] pros = t.GetType().GetProperties();
                var pro = pros.FirstOrDefault(x => x.Name == filed);
                if (pro == null)
                {
                    return default(TResult);
                }
                return (TResult)pro.GetValue(t);
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Error, ex.Message);
                return default(TResult);
            }
        }

        public static bool SetValue<T>(this T t, string filed, object value) where T : BaseEntity
        {
            try
            {
                PropertyInfo[] pros = t.GetType().GetProperties();
                var pro = pros.FirstOrDefault(x => x.Name == filed);
                if (pro == null)
                {
                    return false;
                }
                pro.SetValue(t, value);
                return true;
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Error, ex.Message);
                return false;
            }

        }

        public static bool Refresh<T>(this T t, string dbid = "") where T : BaseEntity
        {

            if (t == null)
                throw new Exception("实列对象不可为空");

            try
            {
                dbid = Tools.GetDBID(dbid);
                var nea = t.GetNEA();
                var primaryKeys = t.GetPrimaryKey();
                StringBuilder sb_pkwhere = new StringBuilder();
                foreach (var item in primaryKeys)
                {
                    sb_pkwhere.Append($"{nea._Lr}{item.Key}{nea._Rr} = '{item.Value}',");
                }

                var paramSql = new ParamSql($" select {t.GetBaseFieldsStr()} from {nea.ToSqlDBName(dbid)}.{nea.TableName} where {sb_pkwhere.ToString().TrimEnd(',')}");
                var tResult = DBFactory.Instance.Query<T>(dbid, paramSql.SqlStr, paramSql.Params).FirstOrDefault();
                if (tResult != null)
                {
                    var fields = t.GetBaseFieldsStr().Split(',');
                    foreach (var item in fields)
                    {
                        t.SetValue(item, tResult.GetValue<T, object>(item));
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                SqlLogUtil.SendLog(LogType.Error, ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 比较实体对象返回两个对象变更的字段(第一个返回是自己的数值，第二个返回是对比的对象数值)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="source"></param>
        /// <param name="aliasName"></param>
        /// <returns></returns>
        public static (object, object) CompareChange<T>(this T self, T source, bool aliasName = false) where T : BaseEntity
        {

            IDictionary<string, object> f = new ExpandoObject();
            IDictionary<string, object> s = new ExpandoObject();

            PropertyInfo[] pros = self.GetType().GetProperties();

            foreach (var pro in pros)
            {
                if (pro.CanRead && pro.CanWrite)
                {
                    string fValue = "";
                    string sValue = "";
                    if (pro.PropertyType == typeof(decimal))
                    {
                        fValue = ((decimal)pro.GetValue(self)).ToString("#.##############");
                        sValue = ((decimal)pro.GetValue(source)).ToString("#.##############");
                    }
                    else
                    {
                        fValue = pro.GetValue(self)?.ToString();
                        sValue = pro.GetValue(source)?.ToString();
                    }

                    if (fValue != sValue)
                    {
                        string keyName = pro.Name;
                        if (aliasName)
                        {
                            var epa = (QingPropertyAttribute)Attribute.GetCustomAttribute(pro, typeof(QingPropertyAttribute));
                            if (!string.IsNullOrEmpty(epa?.Alias))
                            {
                                keyName = epa.Alias;
                            }

                        }
                        f[keyName] = fValue;
                        s[keyName] = sValue;

                    }
                }
            }

            if (f.Count > 0)
            {

                return (f, s);
            }
            else
            {
                return (null, null);
            }
        }

        /// <summary>
        /// 主键更新与原(Source)对象对比有变更的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="context"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int CompareChangeToUpdateByPrimaryKey<T>(this T self, DBContext context, T source) where T : BaseEntity
        {
            var ps = CreateCompareChangeSqlByPrimaryKey(self, source, context.CurrentDB);
            if (ps.SqlStr != null)
            {
                return context.Execute(ps.SqlStr, ps.Params);
            }
            else
            {
                return 0;
            }
        }

        public static Task<int> CompareChangeToUpdateByPrimaryKeyAsync<T>(this T self, DBContext context, T source) where T : BaseEntity
        {
            var ps = CreateCompareChangeSqlByPrimaryKey(self, source, context.CurrentDB);
            if (ps.SqlStr != null)
            {
                return context.ExecuteAsync(ps.SqlStr, ps.Params);
            }
            else
            {
                return Task.FromResult(0);
            }
        }
        /// <summary>
        /// 条件更新与原(Source)对象对比有变更的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int CompareChangeToUpdate<T>(this T self, DBContext context, T source, Expression<Func<T, bool>> where) where T : BaseEntity
        {
            var ps = CreateCompareChangeSql(self, source, context.CurrentDB, where);
            if (ps.SqlStr != null)
            {
                return context.Execute(ps.SqlStr, ps.Params);
            }
            else
            {
                return 0;
            }
        }

        public static Task<int> CompareChangeToUpdateAsync<T>(this T self, DBContext context, T source, Expression<Func<T, bool>> where) where T : BaseEntity
        {
            var ps = CreateCompareChangeSql(self, source, context.CurrentDB, where);
            if (ps.SqlStr != null)
            {
                return context.ExecuteAsync(ps.SqlStr, ps.Params);
            }
            else
            {
                return Task.FromResult(0);
            }
        }
        /// <summary>
        /// 事务主键更新与原(Source)对象对比有变更的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int CompareChangeToUpdateByPrimaryKey<T>(this T self, IDbTransaction transaction, T source) where T : BaseEntity
        {
            var ps = CreateCompareChangeSqlByPrimaryKey(self, source, transaction.Connection.Database);
            if (ps.SqlStr != null)
            {
                return transaction.Connection.Execute(ps.SqlStr, ps.Params);
            }
            else
            {
                return 0;
            }
        }

        public static Task<int> CompareChangeToUpdateByPrimaryKeyAsync<T>(this T self, IDbTransaction transaction, T source) where T : BaseEntity
        {
            var ps = CreateCompareChangeSqlByPrimaryKey(self, source, transaction.Connection.Database);
            if (ps.SqlStr != null)
            {
                return transaction.Connection.ExecuteAsync(ps.SqlStr, ps.Params);
            }
            else
            {
                return Task.FromResult(0);
            }
        }
        /// <summary>
        /// 事务条件更新与原(Source)对象对比有变更的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="context"></param>
        /// <param name="source"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static int CompareChangeToUpdate<T>(this T self, IDbTransaction transaction, T source, Expression<Func<T, bool>> where) where T : BaseEntity
        {
            var ps = CreateCompareChangeSql(self, source, transaction.Connection.Database, where);
            if (ps.SqlStr != null)
            {
                return transaction.Connection.Execute(ps.SqlStr, ps.Params);
            }
            else
            {
                return 0;
            }
        }

        public static Task<int> CompareChangeToUpdateAsync<T>(this T self, IDbTransaction transaction, T source, Expression<Func<T, bool>> where) where T : BaseEntity
        {
            var ps = CreateCompareChangeSql(self, source, transaction.Connection.Database, where);
            if (ps.SqlStr != null)
            {
                return transaction.Connection.ExecuteAsync(ps.SqlStr, ps.Params);
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        internal static ParamSql CreateCompareChangeSqlByPrimaryKey<T>(T self, T source, string dbID) where T : BaseEntity
        {
            bool changed = false;
            ParamSql ps = new ParamSql();
            Dictionary<string, string> Primarys = new Dictionary<string, string>();
            var nea = self.GetNEA();
            var primaryKeys = nea.PrimaryKey.Split(',').ToList();
            StringBuilder sb_SQL = new StringBuilder(string.Format($"Update {nea.ToSqlDBName(dbID)}.{nea.TableName} set "));


            PropertyInfo[] pros = self.GetType().GetProperties();
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {
                    if (primaryKeys.Contains(item.Name))
                    {

                        var value = item.GetValue(self, null).ToString();
                        Primarys[item.Name] = value;
                        continue;
                    }

                    if (item.PropertyType == typeof(DateTime))
                    {
                        var selfValue = Convert.ToDateTime(item.GetValue(self)).ToString("yyyy-MM-dd HH:mm:ss");
                        var sourceValue = Convert.ToDateTime(item.GetValue(source)).ToString("yyyy-MM-dd HH:mm:ss");
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                    else if (item.PropertyType == typeof(decimal))
                    {
                        var selfValue = Convert.ToDecimal(item.GetValue(self)).ToString("#.###################");
                        var sourceValue = Convert.ToDecimal(item.GetValue(source)).ToString("#.###################");
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                    else
                    {
                        var selfValue = item.GetValue(self)?.ToString();
                        var sourceValue = item.GetValue(source)?.ToString();
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {

                StringBuilder whereStr = new StringBuilder(" where 1=1 ");
                foreach (var item in Primarys)
                {
                    whereStr.Append($"and {nea._Lr}{item.Key}{nea._Rr}=@{item.Key}_PK ");
                    ps.Params.Add(item.Key + "_PK", item.Value);
                }
                ps.SqlStr = sb_SQL.ToString().TrimEnd(',') + whereStr.ToString() + ";";
            }
            else
            {
                ps.SqlStr = null;
            }

            return ps;
        }
        internal static ParamSql CreateCompareChangeSql<T>(T self, T source, string dbID, Expression<Func<T, bool>> where) where T : BaseEntity
        {
            bool changed = false;
            ParamSql ps = new ParamSql();
            var nea = self.GetNEA();
            StringBuilder sb_SQL = new StringBuilder(string.Format($"Update {nea.ToSqlDBName(dbID)}.{nea.TableName} set "));


            PropertyInfo[] pros = self.GetType().GetProperties();
            foreach (var item in pros)
            {
                if (item.CanRead && item.CanWrite)
                {

                    if (item.PropertyType == typeof(DateTime))
                    {
                        var selfValue = Convert.ToDateTime(item.GetValue(self)).ToString("yyyy-MM-dd HH:mm:ss");
                        var sourceValue = Convert.ToDateTime(item.GetValue(source)).ToString("yyyy-MM-dd HH:mm:ss");
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                    else if (item.PropertyType == typeof(decimal))
                    {
                        var selfValue = Convert.ToDecimal(item.GetValue(self)).ToString("#.###################");
                        var sourceValue = Convert.ToDecimal(item.GetValue(source)).ToString("#.###################");
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                    else
                    {
                        var selfValue = item.GetValue(self)?.ToString();
                        var sourceValue = item.GetValue(source)?.ToString();
                        if (selfValue != sourceValue)
                        {
                            sb_SQL.Append($"{nea._Lr}{item.Name}{nea._Rr}=@{item.Name},");
                            ps.Params.Add(item.Name, selfValue);
                            changed = true;
                        }
                    }
                }
            }
            if (changed)
            {
                var sqlWhere = ExpressionResolver.ResoveExpression(where.Body);
                ps.Params.AddDynamicParams(sqlWhere.Params);
                ps.SqlStr = sb_SQL.ToString().TrimEnd(',') + " where " + sqlWhere.SqlStr + ";";
            }
            else
            {
                ps.SqlStr = null;
            }

            return ps;
        }
    }
}
