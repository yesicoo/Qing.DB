using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Model
{
    public class DBConnInfo
    {
        public string Tag { set; get; }
        public string DBType { set; get; }
        public string DBConnStr { set; get; }
        public string DefaultDBName { get; internal set; }
    }
}
