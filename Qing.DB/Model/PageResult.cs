using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qing.DB.Model
{
   public class PageResult<T>
    {
        public int TotalCount { set; get; }
        public int CurrentPageNum { set; get; }
        public int CurrentPageSize { set; get; }
        public int TotalPages { get { return CurrentPageSize>0?((TotalCount + CurrentPageSize - 1) / CurrentPageSize):0; } }
        public IEnumerable<T> ArrayData { set; get; }
    }
}
