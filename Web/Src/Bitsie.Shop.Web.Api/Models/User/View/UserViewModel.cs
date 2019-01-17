using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UserViewModel : BaseViewModel
    {
        #region Fields

        protected readonly User InnerUser;

        #endregion

        #region Constructor

        public UserViewModel(User user)
        {
            InnerUser = user;
        }

        #endregion

        #region Properties

        public int UserId { get { return InnerUser.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string FirstName { get { return InnerUser.FirstName; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string LastName { get { return InnerUser.LastName; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string MerchantId { get { return InnerUser.MerchantId; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Email { get { return InnerUser.Email; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Phone { get { return InnerUser.Company == null ? "" : InnerUser.Company.Phone; } }
        public UserStatus Status { get { return InnerUser.Status; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Role { get { return InnerUser.Role.GetDescription(); } }
        public bool EnableGratuity { get { return InnerUser.Settings.EnableGratuity; } }

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string BackgroundColor { get { return InnerUser.Settings.BackgroundColor; } }
        public string LogoUrl { get { return InnerUser.Settings.LogoUrl; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string StoreTitle { get { return InnerUser.Settings.StoreTitle; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string HtmlTemplate { get { return InnerUser.Settings.HtmlTemplate; } }

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CompanyName { get { return InnerUser.Company == null ? "" : InnerUser.Company.Name; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Street { get { return InnerUser.Company == null ? "" : InnerUser.Company.Street; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Street2 { get { return InnerUser.Company == null ? "" : InnerUser.Company.Street2; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string City { get { return InnerUser.Company == null ? "" : InnerUser.Company.City; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string State { get { return InnerUser.Company == null ? "" : InnerUser.Company.State; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Zip { get { return InnerUser.Company == null ? "" : InnerUser.Company.Zip; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Website { get { return InnerUser.Company == null ? "" : InnerUser.Company.Website; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Industry { get { return InnerUser.Company == null ? "" : InnerUser.Company.Industry; } }

        public bool EnableFreshbooks { get { return !string.IsNullOrEmpty(InnerUser.Settings.FreshbooksApiUrl); } }
        public PaymentMethod? PaymentMethod { get { return InnerUser.Settings.PaymentMethod; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PaymentAddress { get { return InnerUser.Settings.PaymentAddress; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PublicMasterKey { get { return InnerUser.Wallet == null ? null : InnerUser.Wallet.PublicMasterKey; } }
        public bool SavedPassphrase { get { return InnerUser.Wallet == null || string.IsNullOrEmpty(InnerUser.Wallet.EncryptedPrivateMasterKey) ? false : true; } }

        #endregion


    }
}