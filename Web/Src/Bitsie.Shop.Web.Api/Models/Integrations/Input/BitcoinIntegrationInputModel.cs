using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class BitcoinIntegrationInputModel
    {
        public string PaymentAddress { get; set; }
        
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            if (!String.IsNullOrEmpty(PaymentAddress) && !Regex.IsMatch(PaymentAddress, "^[13][a-zA-Z0-9]{26,33}$"))
            {
                validationDictionary.AddError("PaymentAddress", "Invalid bitcoin address.");
            }
            return validationDictionary.Errors.Count == 0;
        }
    }
}