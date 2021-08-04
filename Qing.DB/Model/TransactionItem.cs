using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Model
{
   public class TransactionItem
    {
        public string TID { set; get; }
        public IDbTransaction DbTransaction { set; get; }
        public DBConnectionItem ConnectionItem { set; get; }
    }
}
