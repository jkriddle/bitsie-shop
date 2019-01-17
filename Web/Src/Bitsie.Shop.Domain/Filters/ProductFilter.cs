using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class ProductFilter : BaseFilter
    {
        public ProductFilter()
        {
            SortColumn = "Title";
        }

        public int? UserId { get; set; }
        public string Title { get; set; }
        public string MerchantId { get; set; }
        public ProductStatus? Status { get; set; }
    }
}
