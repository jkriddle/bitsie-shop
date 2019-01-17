using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bitsie.Shop.Web.Attributes;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class ProductController : BaseMerchantController
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IConfigService _configService;

        public ProductController(IUserService userService, IOrderService orderService, IConfigService configService,
            IProductService productService) : base(userService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _configService = configService;
        }

        public new ActionResult Index(string id)
        {
            var merchant = GetMerchant(id);

            var vm = new BaseMerchantViewModel
            {
                Merchant = merchant
            };
            return MerchantView(merchant, "Index", vm);
        }

        public ActionResult Buy(string id, int item)
        {
            var product = _productService.GetProductById(item);
            if (product == null) throw new HttpException(404, "Product not found.");
            if (product.User.MerchantId != id) throw new HttpException(404, "Product not found.");

            var order = new Order
            {
                Description = "",
                Gratuity = 0,
                Subtotal = product.Price,
                Total = product.Price,
                OrderDate = DateTime.UtcNow,
                OrderType = OrderType.HostedCheckout,
                User = product.User,
                Product = product,
                Customer = (product.User == CurrentUser ? null : CurrentUser)
            };

            var validationState = new ValidationDictionary();
            if (_orderService.ValidateOrder(order, validationState))
            {
                _orderService.CreateOrder(order, _configService.AppSettings("IPNCallbackUrl") + order.User.Settings.PaymentMethod.ToString());
            }

            return Redirect("/" + id + "/checkout?orderNumber=" + order.OrderNumber);
        }

       

        public new ActionResult View(string id, int item)
        {
            var merchant = GetMerchant(id);
            var product = _productService.GetProductById(item);

            if (product.User != merchant) throw new HttpException(404, "Product not found " + item + " for merchant " + id);

            var vm = new Bitsie.Shop.Web.Models.ProductViewModel {
                Product = product,
                Merchant = merchant
            };
            return MerchantView(merchant, "View", vm);
        }

    }
}