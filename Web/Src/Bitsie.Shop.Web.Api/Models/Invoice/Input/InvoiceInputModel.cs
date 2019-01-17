using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Type;

namespace Bitsie.Shop.Web.Api.Models
{
    public class InvoiceInputModel
    {
        public string InvoiceNumber { get; set; }        
        public DateTime DueDate { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public decimal USDAmount { get; set; }
        public int CustomerId { get; set; }
        public string Description { get; set; }
        public List<InvoiceItem> InvoiceItems { get; set; }

        public bool ValidateRequest(IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
            if (USDAmount <= 0)
            {
                requestDictionary.AddError("Subtotal", "Subtotal must be greater than 0.");
            }
                        
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }
    }
}