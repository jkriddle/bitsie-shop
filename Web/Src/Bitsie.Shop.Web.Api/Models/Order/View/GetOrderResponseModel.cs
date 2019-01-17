using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class GetOrderResponseModel : BaseResponseModel
    {
        public OrderViewModel Order { get; set; }
    }
}