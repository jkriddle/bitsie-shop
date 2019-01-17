using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class CheckoutController : BaseMerchantController
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IConfigService _configService;
        private readonly IFreshbooksService _freshbooksService;
        private readonly IInvoiceService _invoiceService;
        private readonly ISubscriptionService _subscriptionService;

        public CheckoutController(IUserService userService, IOrderService orderService, 
            IInvoiceService invoiceService,
            IConfigService configService,
            IFreshbooksService freshbooksService,
            ISubscriptionService subscriptionService)
            : base(userService)
        {
            _userService = userService;
            _orderService = orderService;
            _configService = configService;
            _freshbooksService = freshbooksService;
            _invoiceService = invoiceService;
            _subscriptionService = subscriptionService;
        }

        public ActionResult Index(string id, string orderNumber, string invoice, string lastName, 
                                    decimal? subtotal = null, decimal? gratuity = null, 
                                    string description = null)
        {
            var merchant = GetMerchant(id);

            var vm = new CheckoutViewModel
            {
                Merchant = merchant
            };

            if (!merchant.Settings.PaymentMethod.HasValue 
                || (merchant.Status == UserStatus.Pending || merchant.Status == UserStatus.AwaitingApproval))
            {
                return MerchantView(merchant, "Disabled", vm);
            }

            if (!String.IsNullOrEmpty(invoice))
            {
                if (merchant.Settings.FreshbooksAuthToken != null && !String.IsNullOrEmpty(lastName))
                {
                    return ProcessFreshbooks(merchant, invoice, lastName);    
                }else
                {
                    vm.Invoice = _invoiceService.GetInvoiceByGuid(invoice);
                }
                
            }
            
            if (!String.IsNullOrEmpty(orderNumber))
            {
                vm.Order = _orderService.GetOrderByNumber(orderNumber);
                if (vm.Order == null)
                {
                    throw new HttpException(404, "Order not found.");
                }
            }

            if (subtotal.HasValue)
            {

                if (gratuity == null)
                {
                    gratuity = 0;
                }
                if (subtotal.Value <= 0) throw new HttpException(500, "Invalid subtotal amount.");
                if (gratuity.Value < 0) throw new HttpException(500, "Invalid gratuity amount.");

                var order = new Order
                {
                    Invoice = vm.Invoice,
                    Description = description,
                    Gratuity = gratuity.Value,
                    Subtotal = subtotal.Value,
                    Total = gratuity.Value + subtotal.Value,
                    OrderDate = DateTime.UtcNow,
                    OrderType = OrderType.HostedCheckout,
                    User = merchant,
                    Customer = (merchant == CurrentUser ? null : CurrentUser)
                };

                var validationState = new ValidationDictionary();
                if (_orderService.ValidateOrder(order, validationState))
                {
                    try
                    {
                        _orderService.CreateOrder(order, _configService.AppSettings("IPNCallbackUrl") + order.User.Settings.PaymentMethod.ToString());
                        // Redirect to order page
                        return Redirect("/" + merchant.MerchantId + "/checkout?orderNumber=" + order.OrderNumber);
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

            }

            vm.IsMobile = Request.QueryString["m"] != null;            
            return MerchantView(merchant, "Index", vm);
        }

        public ActionResult Continue(string orderNumber)
        {
            var order = _orderService.GetOrderByNumber(orderNumber);
            if (order == null) throw new HttpException(404, "Order not found.");
            if (order.Product == null) throw new HttpException(404, "Product not found.");

            if (order.Status != OrderStatus.Complete
                && order.Status != OrderStatus.Confirmed
                && order.Status != OrderStatus.Paid) throw new HttpException(401, "Not authorized to access this file.");

            return Redirect(order.Product.RedirectUrl);
        }

        [HttpPost, NoCache, ValidateInput(false)]
        public ActionResult Preview(PreviewInputModel inputModel)
        {
            var vm = new CheckoutViewModel
            {
                Merchant = new User
                {
                    Settings = new Settings
                    {
                        BackgroundColor = inputModel.BackgroundColor,
                        HtmlTemplate = inputModel.HtmlTemplate,
                        StoreTitle = inputModel.StoreTitle,
                        LogoUrl = inputModel.LogoUrl
                    }
                },
                IsMobile = false
            };
            return MerchantView(vm.Merchant, "Preview", vm);
        }

        public ActionResult Subscribe(SubscriptionType plan)
        {
            if (CurrentUser == null)
            {
                return RedirectToAction("Start", "User", new { plan = plan, area="" });
            }

            decimal price = Decimal.Parse(_configService.AppSettings("Subscription." + plan + ".Price"));

            // Add new subscription if necessary
            Subscription subscription = subscription = new Subscription
            {
                Type = plan,
                Price = price,
                DateSubscribed = DateTime.UtcNow,
                DateRenewed = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow,
                Term = SubscriptionTerm.Monthly,
                Status = SubscriptionStatus.Pending
            };
            _subscriptionService.Save(subscription);

            // Generate order and send to checkout
            var merchant = _userService.GetUserByMerchantId("bitsie");
            var order = new Order
            {
                Description = "Bitsie Shop Subscription: " + plan + " Plan",
                Gratuity = 0,
                Subtotal = price,
                Total = price,
                OrderDate = DateTime.UtcNow,
                OrderType = OrderType.Subscription,
                Status = OrderStatus.Pending,
                Subscription = subscription,
                User = merchant,
                Customer = (merchant == CurrentUser ? null : CurrentUser)
            };

            var validationState = new ValidationDictionary();
            if (_orderService.ValidateOrder(order, validationState))
            {
                _orderService.CreateOrder(order, _configService.AppSettings("IPNCallbackUrl") + order.User.Settings.PaymentMethod.ToString());
            }

            return Redirect("/bitsie/checkout?orderNumber=" + order.OrderNumber);
        }

        #region Private Helpers

        /// <summary>
        /// Process a freshbooks order request
        /// </summary>
        /// <param name="merchant"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        private ActionResult ProcessFreshbooks(User merchant, string invoiceNumber, string lastName)
        {
            FreshbooksInvoice invoice;
            _freshbooksService.SetAccount(merchant);
            invoice = _freshbooksService.GetInvoiceByNumber(invoiceNumber);

            if (String.Compare(invoice.LastName, lastName, true) != 0)
            {
                throw new HttpException(404, "Last name does not match name on invoice.");
            }

            var order = new Order
            {
                Description = "Freshbooks Invoice " + invoiceNumber,
                Gratuity = 0,
                Subtotal = Convert.ToDecimal(invoice.Amount),
                Total = Convert.ToDecimal(invoice.Amount),
                OrderDate = DateTime.UtcNow,
                OrderType = OrderType.HostedCheckout,
                User = merchant,
                FreshbooksInvoiceId = Convert.ToInt64(invoice.InvoiceId)
            };

            //‘disputed’, ‘draft’, ‘sent’, ‘viewed’, ‘paid’, ‘auto-paid’, ‘retry’, ‘failed’ or the special status ‘unpaid’ which will retrieve all invoices with a status of ‘disputed’, ‘sent’, ‘viewed’, ‘retry’ or ‘failed’.
            if (!invoice.IsAvailable)
            {
                var vm = new BaseMerchantViewModel();
                vm.Merchant = merchant;
                vm.IsMobile = Request.QueryString["m"] != null;
                return MerchantView(merchant, "NotAvailable", vm);
            }

            _orderService.CreateOrder(order, _configService.AppSettings("IPNCallbackUrl") + order.User.Settings.PaymentMethod.ToString());

            return Redirect("/" + merchant.MerchantId + "/checkout?orderNumber=" + order.OrderNumber);
        }

        #endregion

    }
}