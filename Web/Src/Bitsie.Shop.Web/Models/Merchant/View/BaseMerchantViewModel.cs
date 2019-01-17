using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Models
{
    public class BaseMerchantViewModel : BaseViewModel
    {
        public bool IsMobile { get; set; }
        public User Merchant { get; set; }
    }
}