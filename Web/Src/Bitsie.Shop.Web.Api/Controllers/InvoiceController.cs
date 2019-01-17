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
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class InvoiceController : BaseApiController
    {
        #region Fields

        private readonly IMapperService _mapper;
        private readonly IInvoiceService _invoiceService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructor

        public InvoiceController(IUserService userService,
            ILogService logService,
            IInvoiceService invoiceService,
            IConfigService configService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
            _invoiceService = invoiceService;
            _configService = configService;
        }

        #endregion

        #region Public Methods

        [HttpPost, NoCache, RequiresApiAuth]
        public BaseResponseModel Create(InvoiceInputModel inputModel)
        {
            var vm = new CreateInvoiceResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            User customer = UserService.GetUserById(inputModel.CustomerId);
            var invoice = new Invoice
            {
                USDAmount = inputModel.USDAmount,
                InvoiceGuid = GuidHelper.Create(12),
                InvoiceNumber = inputModel.InvoiceNumber,
                Customer = customer,                
                DueDate = inputModel.DueDate,
                InvoiceDescription = inputModel.Description,
                Merchant = CurrentUser,
                Status = inputModel.InvoiceStatus
            };

            if (_invoiceService.ValidateInvoice(invoice, validationState))
            {
                try
                {
                    _invoiceService.CreateInvoice(invoice);
                    if(inputModel.InvoiceItems != null)
                    {
                        foreach (InvoiceItem invoiceItem in inputModel.InvoiceItems)
                        {
                            invoiceItem.Invoice = invoice;
                        }

                        _invoiceService.SaveInvoiceItems(inputModel.InvoiceItems);
                        invoice.InvoiceItem = inputModel.InvoiceItems;
                    }
                 
                    vm.Invoice = new InvoiceViewModel(invoice);
                    
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
        /// Update invoice information.
        /// </summary>
        /// <param name="inputModel">Info to update</param>
        /// <returns></returns>
        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Update(UpdateInvoiceInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            //get customer to update
            Invoice invoice = _invoiceService.GetInvoiceByGuid(inputModel.InvoiceId);
            if (invoice == null)
            {
                throw new HttpException(404, "Invoice not Found");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditCustomers)
                && invoice.Merchant.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            // Copy properties
            invoice.DueDate = inputModel.DueDate;
            invoice.Customer = UserService.GetUserById(inputModel.CustomerId);
            invoice.InvoiceNumber = inputModel.InvoiceNumber;
            invoice.InvoiceDescription = inputModel.Description;
            invoice.USDAmount = inputModel.USDAmount;
            
            if (_invoiceService.ValidateInvoice(invoice, validationState))
            {
                _invoiceService.UpdateInvoice(invoice);
                _invoiceService.RemoveInvoiceItems(invoice);
                foreach(InvoiceItem invoiceItem in inputModel.InvoiceItems)
                {
                    invoiceItem.Invoice = invoice;
                }
                _invoiceService.SaveInvoiceItems(inputModel.InvoiceItems);

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "Invoice " + inputModel.InvoiceNumber + " (ID #" + invoice.Id + ") was updated.",
                    User = CurrentUser
                });

                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        /// <summary>
        /// Retrieve a single invoice by Guid
        /// </summary>
        /// <param name="id">Invoices Guid</param>
        /// <returns>Invoice data</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public GetInvoiceResponseModel GetOne(string id)
        {
            var invoice = _invoiceService.GetInvoiceByGuid(id);
            if (invoice == null)
            {
                throw new HttpException(404, "Invoice not found.");
            }

            // Do not allow editing of orders other than your own if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditOrders)
                && invoice.Merchant.Id != CurrentUser.Id)
            {
               
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            var vm = new GetInvoiceResponseModel();
            vm.Invoice = new InvoiceViewModel(invoice);
            return vm;
        }


        /// <summary>
        /// Send an invoice to a customer
        /// </summary>
        /// <param name="id">Order's ID</param>
        /// <returns>Order data</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public BaseResponseModel Send(string id)
        {
            var vm = new BaseResponseModel();

            var invoice = _invoiceService.GetInvoiceByGuid(id);
            if (invoice == null)
            {
                throw new HttpException(404, "Invoice not found.");
            }

            // Do not allow editing of orders other than your own if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditOrders)
                && invoice.Customer.Merchant.Id != CurrentUser.Id)
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            //@TODO add error handling for sending invoice
            _invoiceService.SendInvoice(invoice.Id);

            invoice.SendDate = DateTime.Now;
            _invoiceService.UpdateInvoice(invoice);
            vm.Success = true;
            
            return vm;

        }

        /// <summary>
        /// Get the latest invoice number
        /// </summary>
        /// <returns>Count of User's invoices</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public int GetInvoiceCount()
        {
            var filter = new InvoiceFilter();
            filter.MerchantId = CurrentUser.Id;
            var invoices = _invoiceService.GetInvoices(filter, 0, 10);
            return invoices.TotalCount;

        }

        /// <summary>
        /// Retrieve a list of Invoices based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of logs</returns>
        [HttpGet, RequiresApiAuth, NoCache]
        public InvoiceListViewModel Get([FromUri]InvoiceListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new InvoiceListInputModel();

            var filter = new InvoiceFilter();
            _mapper.Map(inputModel, filter);

            if (CurrentUser.Role != Role.Administrator)
            {
                filter.MerchantId = CurrentUser.Id;
            }

            var invoices = _invoiceService.GetInvoices(filter, inputModel.CurrentPage, inputModel.NumPerPage);         
            return new InvoiceListViewModel(invoices);
        }

        #endregion
    }

}
