using System.Collections.Generic;
using Bitsie.Shop.Services;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class ProductListViewModel : PagedViewModel<Product>
    {
        #region Constructor

        public ProductListViewModel(IPagedList<Product> products)
            : base(products)
        {
            Products = new List<ProductViewModel>();
            foreach (Product product in products.Items)
            {
                Products.Add(new ProductViewModel(product));
            }
        }

        #endregion

        #region Public Properties

        public IList<ProductViewModel> Products { get; set; }

        #endregion
    }
}