using System.Collections.Generic;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Models;
using System;
using Bitsie.Shop.Web.Api.Attributes;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Api.Models
{
    public class InvoiceItemViewModel : BaseViewModel
    {
        #region Fields

        private readonly InvoiceItem _innerInvoiceItem;

        #endregion

        #region Constructor

        public InvoiceItemViewModel(InvoiceItem invoiceItem)
        {
            _innerInvoiceItem = invoiceItem;
        }

        #endregion

        #region Properties

        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Description { get { return _innerInvoiceItem.Description; } }
        public decimal UsdAmount { get { return _innerInvoiceItem.UsdAmount; } }
        public int Quantity { get { return _innerInvoiceItem.Quantity; } }
        public int Position { get { return _innerInvoiceItem.Position; } }

        #endregion


    }
}