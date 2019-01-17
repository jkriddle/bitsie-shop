using System.Collections.Generic;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Models;
using System;
using Newtonsoft.Json;
using Bitsie.Shop.Web.Api.Attributes;

namespace Bitsie.Shop.Web.Api.Models
{
    public class InvoiceViewModel : BaseViewModel
    {
        #region Fields

        private readonly Invoice _innerInvoice;

        #endregion

        #region Constructor

        public InvoiceViewModel(Invoice invoice)
        {
            _innerInvoice = invoice;
        }

        #endregion

        #region Properties

        public int InvoiceId { get { return _innerInvoice.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string InvoiceNumber { get { return _innerInvoice.InvoiceNumber; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string InvoiceGuid { get { return _innerInvoice.InvoiceGuid; } }
        public decimal USDAmount { get { return _innerInvoice.USDAmount; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string SendDate { get { return _innerInvoice.SendDate.ToString(); } }
        [JsonConverter(typeof(SanitizeXssConverter))]    
        public string DueDate { get { return _innerInvoice.DueDate.ToString("o"); } }        
        public InvoiceStatus Status { get { return _innerInvoice.Status; } }
        public int? CustomerId { get { return _innerInvoice.Customer.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Description { get { return _innerInvoice.InvoiceDescription; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string MerchantId { get { return _innerInvoice.Merchant.MerchantId; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CustomerFirstName { get { return _innerInvoice.Customer.FirstName; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string CustomerLastName { get { return _innerInvoice.Customer.LastName; } }
        public IList<InvoiceItemViewModel> InvoiceItems { get
        {
            IList<InvoiceItemViewModel> invoiceItemList = new List<InvoiceItemViewModel>();
            if(_innerInvoice.InvoiceItem != null)
            {
               
                foreach (InvoiceItem invoiceItem in _innerInvoice.InvoiceItem)
                {
                    InvoiceItemViewModel invoiceItemViewModel = new InvoiceItemViewModel(invoiceItem);
                    invoiceItemList.Add(invoiceItemViewModel);
                }
               
            }
            return invoiceItemList;
           
        } }

        #endregion


    }
}