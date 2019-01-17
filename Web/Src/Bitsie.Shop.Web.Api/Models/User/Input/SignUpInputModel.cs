using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class SignUpInputModel : BaseInputModel
    {
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public virtual string Street { get; set; }
        public virtual string Street2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Website { get; set; }
        public virtual string Industry { get; set; }
        public virtual PaymentMethod? PaymentMethod { get; set; }
        public virtual string BitpayApiKey { get; set; }
        public virtual string CoinbaseApiKey { get; set; }
        public virtual string CoinbaseApiSecret { get; set; }
        public virtual string PaymentAddress { get; set; }
        public virtual bool IsTipsieUser { get; set; }

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
            if (PaymentMethod.HasValue
                && PaymentMethod.Value == Domain.PaymentMethod.Bitpay
                && string.IsNullOrEmpty(BitpayApiKey))
            {
                requestDictionary.AddError("BitpayApiKey", "API Key is required.");
            }

            if (PaymentMethod.HasValue
                && PaymentMethod.Value == Domain.PaymentMethod.Coinbase
                && string.IsNullOrEmpty(CoinbaseApiKey))
            {
                requestDictionary.AddError("CoinbaseApiKey", "API Key is required.");
            }

            if (PaymentMethod.HasValue
                && PaymentMethod.Value == Domain.PaymentMethod.Coinbase
                && string.IsNullOrEmpty(CoinbaseApiSecret))
            {
                requestDictionary.AddError("CoinbaseApiSecret", "API Secret is required.");
            }
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}