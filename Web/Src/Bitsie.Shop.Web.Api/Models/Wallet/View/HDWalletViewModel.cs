using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Api.Models
{
    public class HDWalletViewModel : BaseViewModel
    {
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PublicAddress {get; set;}
    }
}