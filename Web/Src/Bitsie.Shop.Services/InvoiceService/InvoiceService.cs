using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NHibernate.Mapping;

namespace Bitsie.Shop.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IConfigService _configService;
        private readonly INotificationService _notificationService;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;

        public InvoiceService(IConfigService configService, 
            INotificationService notificationService,
            IInvoiceItemRepository invoiceItemRepository,
            IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _configService = configService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="invoice"></param>
        public void CreateInvoice(Invoice invoice)
        {
            _invoiceRepository.Save(invoice);
        }


        /// <summary>
        /// Update Invoice information
        /// </summary>
        /// <param name="customer">Invoice to update</param>
        /// <returns></returns>
        public void UpdateInvoice(Invoice invoice)
        {            
            _invoiceRepository.Save(invoice);
           
        }

        /// <summary>
        /// Save invoice data
        /// </summary>
        /// <param name="invoice"></param>
        public void Save(Invoice invoice)
        {
            _invoiceRepository.Save(invoice);
        }

        /// <summary>
        /// Save InvoiceItems
        /// </summary>
        /// <param name="invoiceItemList">List of invoice items</param>
        public void SaveInvoiceItems(List<InvoiceItem> invoiceItemList)
        {
           foreach(InvoiceItem invoiceItem in invoiceItemList)
           {
               _invoiceItemRepository.Save(invoiceItem);   
           }
            
        }

        /// <summary>
        /// Remove Invoice items
        /// </summary>
        /// <param name="invoice">Invoice to remove items from</param>
        public void RemoveInvoiceItems(Invoice invoice)
        {
            var originalInvoice = _invoiceRepository.FindAll().Where(i => i.InvoiceGuid == invoice.InvoiceGuid).FirstOrDefault();
            foreach (InvoiceItem invoiceItem in originalInvoice.InvoiceItem)
            {
                _invoiceItemRepository.Delete(invoiceItem);
            }
            originalInvoice.InvoiceItem.Clear();    
        }
        

        /// <summary>
        /// Retrieve a single invoice
        /// </summary>
        /// <param name="id">Invoice's unique ID</param>
        /// <returns>Matching invoice</returns>
        public Invoice GetInvoiceById(int id)
        {
            return _invoiceRepository.FindOne(id);
        }


        /// <summary>
        /// Send invoice to a customer
        /// </summary>
        /// <param name="id">Invoice Id</param>
        public void SendInvoice(int id)
        {
            InvoiceFilter invoiceFilter = new InvoiceFilter();
            invoiceFilter.InvoiceId = id;

            Invoice invoice = GetInvoices(invoiceFilter, 1,1).Items.First();
            invoice.USDAmount = Math.Round(invoice.USDAmount, 2);
            var date = invoice.DueDate.ToString("MM/dd/yyyy");
            invoice.DueDate = DateTime.ParseExact(date, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
            
            _notificationService.Notify(invoice.Customer.Email,
            "You have received an invoice from " + invoice.Customer.Merchant.Company.Name,
            "Invoice", invoice);    
            
        }


        /// <summary>
        /// Retrieve a single invoice
        /// </summary>
        /// <param name="id">Invoice's unique number</param>
        /// <returns>Matching Invoice</returns>
        public Invoice GetInvoiceByNumber(string number, int merchantId)
        {
            return _invoiceRepository.FindAll().Where(o => o.Merchant.Id == merchantId).FirstOrDefault(o => o.InvoiceGuid == number);
        }

         public IList<Invoice> GetOpenInvoices()
        {
            return _invoiceRepository.FindAll().Where(i => i.Status == InvoiceStatus.Pending 
                || i.Status == InvoiceStatus.Partial
                || i.Status == InvoiceStatus.Paid).ToList();
        }


         /// <summary>
         /// Retrieve an invoice by guid
         /// </summary>
         /// <param name="invoiceGuid">Invoice Guid</param>
         /// <returns></returns>
        public Invoice GetInvoiceByGuid(string invoiceGuid)
         {
             return _invoiceRepository.FindAll().FirstOrDefault(i => i.InvoiceGuid == invoiceGuid);
         }

        /// <summary>
        /// Retrieve a paged list of invoices
        /// </summary>
        /// <param name="filter">Query to filter invoices</param>
        /// <param name="currentPage">Current page number</param>
        /// <param name="numPerPage">Number of items per page</param>
        /// <returns></returns>
        public IPagedList<Invoice> GetInvoices(InvoiceFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<Invoice> invoices= _invoiceRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();       
            return new PagedList<Invoice>(invoices, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Validate an invoice
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="validationDictionary"></param>
        /// <returns></returns>
        public bool ValidateInvoice(Invoice invoice, IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
        
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }

    }
}
