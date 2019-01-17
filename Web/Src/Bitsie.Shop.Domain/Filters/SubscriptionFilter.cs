using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class SubscriptionFilter : BaseFilter
    {
        public DateTime? ExpiresBefore { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public SubscriptionStatus? Status { get; set; }
    }
}
