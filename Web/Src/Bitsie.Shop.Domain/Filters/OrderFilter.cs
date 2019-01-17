using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class OrderFilter : BaseFilter
    {
        public OrderFilter()
        {
            SortColumn = "OrderDate";
        }

        /// <summary>
        /// Only orders after this specific ID
        /// </summary>
        public int? AfterId { get; set; }

        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? OnlyChildren { get; set; }
        public bool? IncludeChildren { get; set; }
    }
}
