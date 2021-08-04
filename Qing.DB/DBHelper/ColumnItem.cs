using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.DBHelper
{
   public class ColumnItem
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Field { set; get; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public string Null { set; get; }
        /// <summary>
        /// 主外键
        /// </summary>
        public string Key { set; get; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string Default { set; get; }
        /// <summary>
        /// 补充信息（包含自增）
        /// </summary>
        public string Extra { set; get; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Comment { set; get; }
    }
}
