using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class InvoiceFilter : BaseFilter
    {
        public InvoiceFilter()
        {
            SortColumn = "InvoiceNumber";
        }

        public int? InvoiceId { get; set; }
        public int? CustomerId { get; set; }
        public int? MerchantId { get; set; }
        public InvoiceStatus? Status { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CustomerLastName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
    }
}
