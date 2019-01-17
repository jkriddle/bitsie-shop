using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;
using System;

namespace Bitsie.Shop.Web.Api.Models
{
    public class SecureOrderViewModel : OrderViewModel
    {
        public SecureOrderViewModel(Order order) : base(order) { }

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CustomerName
        {
            get
            {
                if (!String.IsNullOrEmpty(InnerOrder.User.Company.Name)) return InnerOrder.User.Company.Name;
                else return InnerOrder.User.FirstName + " " + InnerOrder.User.LastName;
            }
        }

    }
}