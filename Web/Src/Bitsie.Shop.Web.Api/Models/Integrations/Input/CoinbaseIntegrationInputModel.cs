using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class CoinbaseIntegrationInputModel
    {
        public string CoinbaseApiKey { get; set; }
        public string CoinbaseApiSecret { get; set; }
        
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            if (!String.IsNullOrEmpty(CoinbaseApiKey) && String.IsNullOrEmpty(CoinbaseApiSecret))
            {
                validationDictionary.AddError("CoinbaseApiSecret", "API Secret is required.");
            }
            else if (!String.IsNullOrEmpty(CoinbaseApiKey) && String.IsNullOrEmpty(CoinbaseApiSecret))
            {
                validationDictionary.AddError("CoinbaseApiKey", "API Key is required.");
            }
            return validationDictionary.Errors.Count == 0;
        }
    }
}