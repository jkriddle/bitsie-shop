using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Bitsie.Shop.Web.Api.Models
{
    public class AdminUserViewModel : UserViewModel
    {
        #region Constructor

        public AdminUserViewModel(User user)
            : base(user)
        {
            OfflineAddresses = new List<OfflineAddressViewModel>();
            foreach(var a in user.OfflineAddresses) {
                OfflineAddresses.Add(new OfflineAddressViewModel(a));
            }
        }

        #endregion

        #region Properties

        public IList<OfflineAddressViewModel> OfflineAddresses { get; set; }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string FreshbooksApiUrl { get { return InnerUser.Settings.FreshbooksApiUrl == null ? "" : InnerUser.Settings.FreshbooksApiUrl; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string FreshbooksAuthToken { get { return InnerUser.Settings.FreshbooksAuthToken == null ? "" : InnerUser.Settings.FreshbooksAuthToken; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PaymentMethodDescription { get { return InnerUser.Settings.PaymentMethod.HasValue ? InnerUser.Settings.PaymentMethod.Value.GetDescription() : ""; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string BitpayApiKey { get { return InnerUser.Settings.BitpayApiKey; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CoinbaseApiKey { get { return InnerUser.Settings.CoinbaseApiKey; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CoinbaseApiSecret { get { return InnerUser.Settings.CoinbaseApiSecret; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string GoCoinAccessToken { get { return InnerUser.Settings.GoCoinAccessToken; } }

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string SubscriptionType { get { return InnerUser.Subscription == null ? "Free" : InnerUser.Subscription.Type.GetDescription(); } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string SubscriptionStatus { get { return InnerUser.Subscription == null ? "Active" : InnerUser.Subscription.Status.GetDescription(); } }
        public DateTime? SubscriptionDateExpires { get { return InnerUser.Subscription == null ? (DateTime?)null : InnerUser.Subscription.DateExpires; } }
        public DateTime? SubscriptionDateRenewed { get { return InnerUser.Subscription == null ? (DateTime?)null : InnerUser.Subscription.DateRenewed; } }
        public int NumTransactions
        {
            get
            {
                if (InnerUser.Subscription == null)
                {
                    return Int32.Parse(ConfigurationManager.AppSettings["Subscription.Starter.Transactions"]);
                }

                return Int32.Parse(ConfigurationManager.AppSettings["Subscription." + InnerUser.Subscription.Type.ToString() + ".Transactions"]);
            }
        }
        
        #endregion


    }
}