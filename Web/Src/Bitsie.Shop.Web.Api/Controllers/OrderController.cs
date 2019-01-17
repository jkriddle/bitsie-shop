using System.Web;
using System.Web.Http;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Domain;
using System;
using Bitsie.Shop.Api;
using System.Linq;
using Bitsie.Shop.Services.CSV;
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class OrderController : BaseApiController
    {
        #region Fields

        private readonly IMapperService _mapper;
        private readonly IOrderService _orderService;
        private readonly IBitcoinService _bitcoinService;
        private readonly IConfigService _configService;
        private readonly IFreshbooksService _freshbooksService;
        private readonly IProductService _productService;
        private readonly IInvoiceService _invoiceService;

        #endregion

        #region Constructor

        public OrderController(IUserService userService,
            ILogService logService,
            IOrderService orderService,
            IBitcoinService bitcoinService,
            IConfigService configService,
            IFreshbooksService freshbooksService,
            IProductService productService,
            IInvoiceService invoiceService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
            _orderService = orderService;
            _bitcoinService = bitcoinService;
            _configService = configService;
            _freshbooksService = freshbooksService;
            _productService = productService;
            _invoiceService = invoiceService;
        }

        #endregion

        #region Public Methods

        [HttpPost, NoCache]
        public BaseResponseModel Create(OrderInputModel inputModel)
        {
            var vm = new CreateOrderResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            var merchant = ApplyMerchantSettings(inputModel.MerchantId);
            if (merchant == null && CurrentUser != null)
            {
                merchant = CurrentUser;
            }
            else if (merchant == null)
            {
                throw new HttpException(404, "Merchant not found.");
            }

            if (merchant.Status == UserStatus.Suspended)
            {
                return new BaseResponseModel
                {
                    Errors = {
                        "This merchant has reached their transaction limits and has been temporarily suspended. Please contact support@bitsie.com."
                    },
                    Success = false
                };
            }

            // Check for Freshbooks invoice if not a bitsie invoice
            ulong? freshbooksId = null;
            if(inputModel.InvoiceNumber == null)
            {
                    if (!String.IsNullOrEmpty(merchant.Settings.FreshbooksAuthToken)
                        && !String.IsNullOrEmpty(inputModel.InvoiceNumber))
                    {
                        FreshbooksInvoice invoice;
                        try
                        {
                            _freshbooksService.SetAccount(merchant);
                            invoice = _freshbooksService.GetInvoiceByNumber(inputModel.InvoiceNumber);
                            freshbooksId = invoice.InvoiceId;
                            if (String.Compare(invoice.LastName, inputModel.LastName, true) != 0)
                            {
                                vm.Errors.Add("Last name does not match name on invoice.");
                                return vm;
                            }

                            //‘disputed’, ‘draft’, ‘sent’, ‘viewed’, ‘paid’, ‘auto-paid’, ‘retry’, ‘failed’ or the special status ‘unpaid’ which will retrieve all invoices with a status of ‘disputed’, ‘sent’, ‘viewed’, ‘retry’ or ‘failed’.
                            if (!invoice.IsAvailable)
                            {
                                vm.Errors.Add("Invoice is not available for payment (already paid or being processed).");
                                return vm;
                            }

                            inputModel.Subtotal = Convert.ToDecimal(invoice.Amount);
                            inputModel.Description = "Freshbooks invoice #" + inputModel.InvoiceNumber;
                        }
                        catch (Exception ex)
                        {
                            vm.Errors.Add(ex.Message);
                            return vm;
                        }
                    }
            }
            var order = new Order
            {
                Description = inputModel.Description,
                Gratuity = inputModel.Gratuity,
                Invoice = _invoiceService.GetInvoiceByNumber(inputModel.InvoiceNumber, merchant.Id),
                Subtotal = inputModel.Subtotal,
                Total = inputModel.Gratuity + inputModel.Subtotal,
                OrderDate = DateTime.UtcNow,
                OrderType = inputModel.OrderType,
                User = merchant,
                FreshbooksInvoiceId = freshbooksId.HasValue ? (long)freshbooksId.Value : (long?)null,
                Customer = (merchant == CurrentUser ? null : CurrentUser)
            };

            // Use a product for required information
            if (inputModel.ProductId.HasValue)
            {
                var product = _productService.GetProductById(inputModel.ProductId.Value);
                if (product == null) throw new HttpException(404, "Product not found.");
                order.Product = product;
                order.Subtotal = product.Price;
                order.Gratuity = 0;
                order.Total = product.Price;
            }

            if (_orderService.ValidateOrder(order, validationState))
            {
                try
                {
                    _orderService.CreateOrder(order, _configService.AppSettings("IPNCallbackUrl") + order.User.Settings.PaymentMethod.ToString());
                    vm.Order = new OrderViewModel(order);
                    vm.Success = true;
                }
                catch (Exception ex)
                {
                    vm.Success = false;
                    vm.Errors.Add(ex.Message + ex.StackTrace + (ex.InnerException == null ? "" : ex.InnerException.Message));
                }
            }
            else
            {
                vm.Errors = validationState.Errors;
            }

            return vm;
        }

        [HttpGet, NoCache]
        public GetOrderResponseModel Update(string id)
        {
            var vm = new GetOrderResponseModel();
            var order = _orderService.GetOrderByNumber(id);
            if (order == null)
            {
                throw new HttpException(404, "Order not found.");
            }
            var completeStatuses = new List<OrderStatus> { 
                OrderStatus.Complete, OrderStatus.Confirmed, OrderStatus.Paid };

            if (!completeStatuses.Contains(order.Status)
                && order.OrderDate.AddMinutes(15) < DateTime.UtcNow)
            {
                order.Status = OrderStatus.Expired;
            }
            vm.Order = new OrderViewModel(order);
            return vm;
        }

        /// <summary>
        /// Retrieve a single order by ID
        /// </summary>
        /// <param name="id">Order's ID</param>
        /// <returns>Order data</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public GetOrderResponseModel GetOne(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                throw new HttpException(404, "Order not found.");
            }

            // Do not allow editing of orders other than your own if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditOrders)
                && order.User.Id != CurrentUser.Id
                && (order.User.Merchant == null || order.User.Merchant.Id != CurrentUser.Id))
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            var vm = new GetOrderResponseModel();
            vm.Order = new OrderViewModel(order);
            return vm;
        }


        /// <summary>
        /// Retrieve a list of logs based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of logs</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public OrderListViewModel Get([FromUri]OrderListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new OrderListInputModel();

            var filter = new OrderFilter();
            _mapper.Map(inputModel, filter);

            if (CurrentUser.Role != Role.Administrator)
            {
                filter.UserId = CurrentUser.Id;
            }

            if (inputModel.Report == "gratuity")
            {
                filter.OnlyChildren = true;
            }

            if (inputModel.Report == "all")
            {
                filter.IncludeChildren = true;
            }

            var orders = _orderService.GetOrders(filter, inputModel.CurrentPage, inputModel.NumPerPage);

            if (inputModel.Export)
            {
                Export("Bitsie_Orders", orders.AllItems);
                return null;
            }

            if (CurrentUser.Role == Role.Administrator) return new SecureOrderListViewModel(orders);
            else return new OrderListViewModel(orders);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Apply settings based on current merchant
        /// </summary>
        /// <param name="merchantId"></param>
        private User ApplyMerchantSettings(string merchantId)
        {
            if (!String.IsNullOrEmpty(merchantId))
            {
                var merchant = UserService.GetUserByMerchantId(merchantId);
                if (merchant == null)
                {
                    throw new HttpException(404, "Merchant not found.");
                }
                return merchant;
            }
            return null;
        }

        #endregion

    }

}
