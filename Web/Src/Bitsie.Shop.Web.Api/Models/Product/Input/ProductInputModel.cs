using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class ProductInputModel
    {
        public int? ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public string RedirectUrl { get; set; }

        public bool ValidateRequest(ValidationDictionary validationDictionary)
        {
            return validationDictionary.Errors.Count == 0;
        }
    }
}