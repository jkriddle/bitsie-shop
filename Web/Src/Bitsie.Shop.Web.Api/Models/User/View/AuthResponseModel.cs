using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Api.Models
{
    public class AuthResponseModel : BaseResponseModel
    {
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public UserViewModel User { get; set; }
    }
}