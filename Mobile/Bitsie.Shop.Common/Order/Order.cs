using System;

namespace Bitsie.Shop.Common
{
	public class Order
	{
		public int OrderId { get; set; }
		public string OrderNumber { get; set; }
		public string PaymentAddress { get; set; }
		public DateTime OrderDate { get; set; }
		public string Description { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public decimal Subtotal { get; set; }
		public decimal Gratuity { get; set; }
		public decimal Total { get; set; }
		public decimal BtcTotal { get; set; }
		public decimal Rate { get; set; }
		public OrderStatus Status { get; set; }
		public string PaymentUrl { get; set; }
		public decimal BtcPaid { get; set; }
		public decimal BtcBalance {
			get {
				return BtcTotal - BtcPaid;
			}
		}
		public decimal UsdBalance {
			get {
				return BtcBalance * Rate;
			}
		}
	}
}

