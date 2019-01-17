using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Models
{
    public class ProductViewModel : BaseViewModel
    {
        #region Fields

        protected readonly Product InnerProduct;

        #endregion

        #region Constructor

        public ProductViewModel(Product product)
        {
            InnerProduct = product;
            if (InnerProduct == null) product = new Product();
        }

        #endregion

        #region Properties

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string MerchantId { get { return InnerProduct.User.MerchantId; } }
        public int ProductId { get { return InnerProduct.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Title { get { return InnerProduct.Title; } }
        public decimal Price { get { return InnerProduct.Price; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Description { get { return InnerProduct.Description; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string ShortDescription { get { return InnerProduct.ShortDescription; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string RedirectUrl { get { return InnerProduct.RedirectUrl; } }
        
        #endregion

    }
}