using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class BaseFilter
    {
        public string SearchTerm { get; set; }
        public string SortColumn { get; set; }
        public SortDirection SortDirection { get; set; }
    }
}
