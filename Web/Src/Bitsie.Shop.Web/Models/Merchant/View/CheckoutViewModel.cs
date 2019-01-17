using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Models
{
    public class CheckoutViewModel : BaseMerchantViewModel
    {
        /// <summary>
        /// Is user viewing checkout page from a mobile generated invoice (i.e. HTML5 app)
        /// </summary>
        public Order Order { get; set; }
        public Invoice Invoice {get; set; }
        public bool FreshbooksEnabled
        {
            get
            {
                return !String.IsNullOrEmpty(Merchant.Settings.FreshbooksAuthToken);
            }
        }
    }
}