using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class Settings : Entity
    {

        /// <summary>
        /// Payout method
        /// </summary>
        public virtual PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// Payment processor API key
        /// </summary>
        public virtual string BitpayApiKey { get; set; }

        /// <summary>
        /// Payment processor API secret
        /// </summary>
        public virtual string CoinbaseApiKey { get; set; }

        /// <summary>
        /// Payment processor API secret
        /// </summary>
        public virtual string CoinbaseApiSecret { get; set; }

        /// <summary>
        /// GoCoin OAuth Code
        /// </summary>
        public virtual string GoCoinAccessToken { get; set; }

        /// <summary>
        /// Custom wallet payment address
        /// </summary>
        public virtual string PaymentAddress { get; set; }

        /// <summary>
        /// Does user allow gratuity at checkout?
        /// </summary>
        public virtual bool EnableGratuity { get; set; }

        /// <summary>
        /// Max amount this merchant can process per day
        /// </summary>
        public virtual decimal DailyMaximum { get; set; }

        /// <summary>
        /// User's freshbooks API Url
        /// </summary>
        public virtual string FreshbooksApiUrl { get; set; }

        /// <summary>
        /// User's freshbooks auth token
        /// </summary>
        public virtual string FreshbooksAuthToken { get; set; }

        public virtual string BackgroundColor { get; set; }
        public virtual string LogoUrl { get; set; }
        public virtual string StoreTitle { get; set; }

        /// <summary>
        /// Custom HTML template for customer pages
        /// </summary>
        public virtual string HtmlTemplate { get; set; }

    }
}
