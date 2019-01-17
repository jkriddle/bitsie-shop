using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OrderInputModel
    {
        public string MerchantId { get; set; }
        public string Description { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Gratuity { get; set; }
        public OrderType OrderType { get; set; }
        public int? ProductId { get; set; }

        /// <summary>
        /// Freshbooks invoice number
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Freshbooks last name
        /// </summary>
        public string LastName { get; set; }

        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();

            if (!ProductId.HasValue && Subtotal <= 0)
            {
                requestDictionary.AddError("Subtotal", "Subtotal must be greater than 0.");
            }
            if (Gratuity < 0)
            {
                requestDictionary.AddError("Total", "Gratuity total must be greater than or equal to 0.");
            }
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}