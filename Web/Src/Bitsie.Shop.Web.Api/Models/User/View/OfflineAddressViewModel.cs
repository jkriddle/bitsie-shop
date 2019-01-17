using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OfflineAddressViewModel
    {
        private readonly OfflineAddress _address;

        public OfflineAddressViewModel(OfflineAddress address)
        {
            _address = address;
        }

        public int Id { get { return _address.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Address { get { return _address.Address; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string EmailNotifications { get { return _address.EmailNotifications; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string TextNotifications { get { return _address.TextNotifications; } }
    }
}