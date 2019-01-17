using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;

namespace Bitsie.Shop.Services
{
    public interface IProductService
    {
        Product GetProductById(int productId);
        IPagedList<Product> GetProducts(ProductFilter filter, int currentPage, int numPerPage);
        bool ValidateProduct(Product product, IValidationDictionary validationDictionary);
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
    }
}
