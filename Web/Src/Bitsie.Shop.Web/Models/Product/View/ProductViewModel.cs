using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Models
{
    public class ProductViewModel : BaseMerchantViewModel
    {
        public Product Product { get; set; }
    }
}