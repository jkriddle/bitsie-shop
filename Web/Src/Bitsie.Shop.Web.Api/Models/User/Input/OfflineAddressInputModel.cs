using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OfflineAddressInputModel : BaseInputModel
    {
        public IList<string> OfflineAddress { get; set; }
        public IList<string> OfflineEmail { get; set; }
        public IList<string> OfflinePhone { get; set; }

        /// <summary>
        /// Validate form-level request
        /// </summary>
        /// <param name="validationDictionary"></param>
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();

            int numAddresses = OfflineAddress == null ? 0 : OfflineAddress.Count;
            for (var i = 0; i < numAddresses; i++)
            {
                string emails = OfflineEmail[i];
                string phones = String.IsNullOrEmpty(OfflinePhone[i]) ? "" : OfflinePhone[i];
                string cleanPhones = Regex.Replace(phones, "[^0-9,]", "");
                
                // Validate emails
                IList<string> records = new List<string>();
                if (emails.Contains(",")) records = emails.Split(',');
                else records.Add(emails);

                for (var j = 0; j < records.Count; j++)
                {
                    if (!String.IsNullOrEmpty(records[i]) && !EmailValidator.IsValid(records[i]))
                    {
                        validationDictionary.AddError("Email" + j, "Invalid offline address email: " + records[i]);
                    }
                }

                // Validate phone numbers
                if (cleanPhones.Contains(",")) records = cleanPhones.Split(',');
                else records.Add(cleanPhones);

                for (var j = 0; j < records.Count; j++)
                {
                    if (!String.IsNullOrEmpty(records[j]) && records[j].Length != 10)
                    {
                        validationDictionary.AddError("Phone" + j, "Invalid offline address phone number: " + records[j]);
                    }
                }
            }

            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}