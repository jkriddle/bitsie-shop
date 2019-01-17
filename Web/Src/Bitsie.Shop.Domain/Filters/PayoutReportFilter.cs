using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class PayoutReportFilter
    {
        public PaymentMethod? PaymentMethod { get; set; }
        public bool AboveMinimumThreshold { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
