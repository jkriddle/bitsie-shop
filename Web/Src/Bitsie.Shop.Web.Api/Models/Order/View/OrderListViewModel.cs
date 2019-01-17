using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OrderListViewModel : PagedViewModel<Order>
    {
        #region Constructor

        public OrderListViewModel(IPagedList<Order> orders)
            : base(orders)
        {
            Orders = new List<OrderViewModel>();
            foreach (var order in orders.Items)
            {
                Orders.Add(new OrderViewModel(order));
            }
        }

        #endregion

        #region Public Properties

        public IList<OrderViewModel> Orders { get; set; }

        public decimal TotalPaid
        {
            get { return Orders.Sum(o => o.Total); }
        }

        // TODO: calculate fee properly based on the user payout settings.
        // We really need a separate Payout/Get call that does this since the only reason
        // These fields are used right now are for the admin payout screen.
        public decimal PayoutFee
        {
            get { return Math.Round(TotalPaid * 0.01m, 2, MidpointRounding.AwayFromZero); }
        }

        public decimal PayoutAmount
        {
            get { return Math.Round(TotalPaid - PayoutFee, 2, MidpointRounding.AwayFromZero); }
        }

        #endregion

    }
}