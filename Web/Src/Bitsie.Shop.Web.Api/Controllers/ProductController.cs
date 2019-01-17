using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Providers;
using System.Text.RegularExpressions;
using System.Linq;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class ProductController : BaseApiController
    {
        #region Fields

        private readonly IMapperService _mapper;
        private readonly IProductService _productService;

        #endregion

        #region Constructor

        public ProductController(IUserService userService, 
            IProductService productService,
            ILogService logService,
            IMapperService mapper) : base(userService, logService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        #endregion

        #region Public Methods

        [HttpPost, NoCache, RequiresApiAuth]
        public CreateProductResponseModel Create(ProductInputModel inputModel)
        {
            var vm = new CreateProductResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            var product = new Product
            {
                Status = ProductStatus.Published,
                Title = inputModel.Title,
                Price = inputModel.Price,
                Description = inputModel.Description,
                ShortDescription = inputModel.ShortDescription,
                User = CurrentUser,
                RedirectUrl = inputModel.RedirectUrl
            };

            if (_productService.ValidateProduct(product, validationState))
            {
                try
                {
                    _productService.CreateProduct(product);
                    vm.ProductId = product.Id;
                    vm.Success = true;
                }
                catch (Exception ex)
                {
                    vm.Success = false;
                    vm.Errors.Add(ex.Message);
                }
            }
            else
            {
                vm.Errors = validationState.Errors;
            }

            return vm;
        }

        [HttpPost, NoCache, RequiresApiAuth]
        public BaseResponseModel Delete(int id)
        {
            var product = _productService.GetProductById(id);

            if (product == null)
            {
                throw new HttpException(404, "Product not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditProducts)
                && product.User.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            var vm = new BaseResponseModel();
            _productService.DeleteProduct(product);
            vm.Success = true;
            return vm;
        }

        [HttpPost, NoCache, RequiresApiAuth]
        public CreateProductResponseModel Update(ProductInputModel inputModel)
        {
            var vm = new CreateProductResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            var product = _productService.GetProductById(inputModel.ProductId.Value);

            if (product == null)
            {
                throw new HttpException(404, "Product not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditProducts)
                && product.User.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            _mapper.Map<ProductInputModel, Product>(inputModel, product);

            if (_productService.ValidateProduct(product, validationState))
            {
                try
                {
                    _productService.UpdateProduct(product);
                    vm.ProductId = product.Id;
                    vm.Success = true;
                }
                catch (Exception ex)
                {
                    vm.Success = false;
                    vm.Errors.Add(ex.Message);
                }
            }
            else
            {
                vm.Errors = validationState.Errors;
            }

            return vm;
        }


        /// <summary>
        /// Retrieve a single product by ID, for public viewing
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>User data</returns>
        [HttpGet, RequiresApiAuth]
        public PublicProductViewModel GetOne(int? id)
        {
            var product = _productService.GetProductById(id.Value);


            if (product == null)
            {
                throw new HttpException(404, "Product not found.");
            }
           
            return new PublicProductViewModel(product);
        }

        /// <summary>
        /// Retrieve a single product by id, for owner view in admin
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>User data</returns>
        [HttpGet, RequiresApiAuth]
        public ProductViewModel GetSecure(int? id)
        {
            var product = _productService.GetProductById(id.Value);

            if (product.User.Id != CurrentUser.Id)
            {

                throw new HttpException(404, "Product not found.");
            }

            if (product == null)
            {
                throw new HttpException(404, "Product not found.");
            }

            return new ProductViewModel(product);
        }

        /// <summary>
        /// Retrieve a list of users based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of users</returns>
        [HttpGet]
        public ProductListViewModel Get([FromUri]ProductListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new ProductListInputModel();

            var filter = new ProductFilter();
            _mapper.Map(inputModel, filter);
            filter.Status = ProductStatus.Published;

            if ((CurrentUser == null || (CurrentUser != null && CurrentUser.Role != Role.Administrator)) 
                && inputModel.MerchantId == null)
            {
                filter.UserId = CurrentUser.Id;
            }

            var products = _productService.GetProducts(filter, inputModel.CurrentPage, inputModel.NumPerPage);

            if (inputModel.Export)
            {
                Export("Bitsie_Products", products.AllItems);
                return null;
            }

            return new ProductListViewModel(products);
        }

        #endregion

    }

}
