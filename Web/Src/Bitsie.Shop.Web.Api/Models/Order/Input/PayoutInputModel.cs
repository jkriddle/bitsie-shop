using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class PayoutInputModel : OrderListInputModel
    {
        public string ConfirmationNumber { get; set; }
        public decimal TotalPayout { get; set; }
    }
}