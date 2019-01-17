using System;
using System.Collections.Generic;
using System.Linq;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;

namespace Bitsie.Shop.Services
{
    public class ProductService : IProductService
    {
        #region Fields

        private readonly IProductRepository _productRepository;

        #endregion

        #region Constructor

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Retrieve product by ID
        /// </summary>
        /// <param name="productId">Product's unique ID</param>
        /// <returns></returns>
        public Product GetProductById(int productId)
        {
            return _productRepository.FindOne(productId);
        }

        /// <summary>
        /// Retrieve a paged list of products
        /// </summary>
        /// <param name="filter">Query to filter products</param>
        /// <param name="currentPage">Current page number</param>
        /// <param name="numPerPage">Number of items per page</param>
        /// <returns></returns>
        public IPagedList<Product> GetProducts(ProductFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<Product> products = _productRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<Product>(products, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Validate a product
        /// </summary>
        /// <param name="product">Product to validate</param>
        /// <param name="validationDictionary">Validation errors</param>
        /// <returns>If product is valid</returns>
        public bool ValidateProduct(Product product, IValidationDictionary validationDictionary)
        {
            if (String.IsNullOrEmpty(product.Title))
            {
                validationDictionary.AddError("Title", "Product title is required.");
            }

            if (product.Price <= 0)
            {
                validationDictionary.AddError("Price", "Price must be greater than 0.");
            }

            if (!String.IsNullOrEmpty(product.Description)) {
                product.Description = HtmlHelper.Sanitize(product.Description);
            }

            return validationDictionary.Errors.Count == 0;
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="product">Product to save</param>
        /// <returns></returns>
        public void CreateProduct(Product product)
        {
            _productRepository.Save(product);
        }

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="product">Product to update</param>
        /// <returns></returns>
        public void UpdateProduct(Product product)
        {
            _productRepository.Save(product);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="product">Product to delete</param>
        /// <returns></returns>
        public void DeleteProduct(Product product)
        {
            product.Status = ProductStatus.Deleted;
            _productRepository.Save(product);
        }

        #endregion


    }
}
