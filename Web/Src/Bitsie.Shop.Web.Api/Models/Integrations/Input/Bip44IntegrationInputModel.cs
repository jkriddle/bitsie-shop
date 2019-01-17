using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class Bip44IntegrationInputModel
    {
        public string MasterPublicKey { get; set; }
        public string EncryptedPrivateKey { get; set; }
        public string walletName { get; set; }
        
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            if (String.IsNullOrEmpty(MasterPublicKey))
            {
                validationDictionary.AddError("MasterPublicKey", "Master public key is required.");
            }
            return validationDictionary.Errors.Count == 0;
        }
    }
}