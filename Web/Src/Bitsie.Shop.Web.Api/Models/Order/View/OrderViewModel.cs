using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;
using System;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OrderViewModel : BaseViewModel
    {
        #region Fields

        protected readonly Order InnerOrder;

        #endregion

        #region Constructor

        public OrderViewModel(Order order)
        {
            InnerOrder = order;
        }

        #endregion

        #region Properties

        public int OrderId { get { return InnerOrder.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string OrderNumber { get { return InnerOrder.OrderNumber; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Description { get { return InnerOrder.Description; } }
        public decimal BtcTotal { get { return InnerOrder.BtcTotal; } }
        public decimal Gratuity { get { return InnerOrder.Gratuity; } }
        public decimal Subtotal { get { return InnerOrder.Subtotal; } }
        public decimal Total { get { return InnerOrder.Total; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string OrderType { get { return InnerOrder.OrderType.GetDescription(); } }
        public decimal BtcPaid { get { return InnerOrder.BtcPaid; } }
        public decimal BtcBalance { get { return InnerOrder.BtcBalance; } }
        public decimal UsdBalance { get { return InnerOrder.UsdBalance; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string OrderDate { get { return InnerOrder.OrderDate.ToString("o"); } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PaymentAddress { get { return InnerOrder.PaymentAddress; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string PaymentUrl { get { return InnerOrder.PaymentUrl; } }
        public decimal Rate { get { return InnerOrder.Rate; } }
        public OrderStatus Status { get { return InnerOrder.Status; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string InvoiceNumber { get { return InnerOrder.Invoice == null ? "" : InnerOrder.Invoice.InvoiceNumber; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string StatusDescription { get { return InnerOrder.Status.GetDescription(); } }
        public int? ProductId { get { return InnerOrder.Product == null ? (int?)null : InnerOrder.Product.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string ProductTitle { get { return InnerOrder.Product == null ? null : InnerOrder.Product.Title; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string FirstName { get { return InnerOrder.User.Merchant == null ? String.Empty : InnerOrder.User.FirstName; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string LastName { get { return InnerOrder.User.Merchant == null ? String.Empty :InnerOrder.User.LastName; } }

        #endregion


    }
}