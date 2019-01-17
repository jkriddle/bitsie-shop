using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class BitpayIntegrationInputModel
    {
        public string BitpayApiKey { get; set; }
        
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            return validationDictionary.Errors.Count == 0;
        }
    }
}