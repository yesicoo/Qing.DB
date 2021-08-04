using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Model
{
   public class SqlFilter
    {
        public List<Where> OrWhere { set; get; } = new List<Where>();
        public List<Where> AndWhere { set; get; } = new List<Where>();
        public List<Order> Orders { set; get; } = new List<Order>();
        public int PageNum { set; get; } = 1;
        public int PageSize { set; get; } = 10;
    }
}
