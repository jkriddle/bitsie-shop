using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Api.Models
{
    public class SecureOrderListViewModel : OrderListViewModel
    {
        #region Constructor

        public SecureOrderListViewModel(IPagedList<Order> orders)
            : base(orders)
        {
            Orders = new List<SecureOrderViewModel>();
            foreach (var order in orders.Items)
            {
                Orders.Add(new SecureOrderViewModel(order));
            }
        }

        #endregion

        #region Public Properties

        public new IList<SecureOrderViewModel> Orders { get; set; }

        #endregion

    }
}