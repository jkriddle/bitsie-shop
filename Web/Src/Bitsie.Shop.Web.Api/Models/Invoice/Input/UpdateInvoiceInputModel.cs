using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using System;
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UpdateInvoiceInputModel : BaseInputModel
    {
        public string InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public decimal USDAmount { get; set; }
        public int CustomerId { get; set; }
        public string Description { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }

        /// <summary>
        /// Validate form-level request
        /// </summary>
        /// <param name="validationDictionary"></param>
        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();           
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}