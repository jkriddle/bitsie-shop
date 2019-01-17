using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UpdateUserInputModel : BaseInputModel
    {
        public enum UpdateType
        {
            Design = 1,
            Profile
        }
        public UpdateType Type { get; set; } 
        public int UserId { get; set; }
        public string MerchantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string BitpayApiKey { get; set; }
        public string CoinbaseApiKey { get; set; }
        public string CoinbaseApiSecret { get; set; }
        public string PaymentAddress { get; set; }
        public string BackupAddress { get; set; }
        public bool? EnableGratuity{ get; set; }
        public Role? Role { get; set; }
        public UserStatus? Status { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string BackgroundColor { get; set; }
        public string StoreTitle { get; set; }
        public string LogoUrl { get; set; }
        public string HtmlTemplate { get; set; }
        public IList<string> OfflineAddress { get; set; }
        public IList<string> OfflineEmail { get; set; }
        public IList<string> OfflinePhone { get; set; }
        public bool? EnableFreshbooks { get; set; }
        public DateTime? SubscriptionDateExpires { get; set; }
        public SubscriptionType? SubscriptionType { get; set; }

        /// <summary>
        /// Validate form-level request
        /// </summary>
        /// <param name="validationDictionary"></param>
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
            if (Password != ConfirmPassword)
            {
                requestDictionary.AddError("Password", "Passwords do not match.");
            }

            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}