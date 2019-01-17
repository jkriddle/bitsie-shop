
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Bitsie.Shop.Common
{
	public class BaseInvoice
	{
		public string Id { get; set; }
		public string Url { get; set; }
		public string Status { get; set; }
		public string BtcPrice { get; set; }
		public decimal Price { get; set; }
		public string Currency { get; set; }
		public string InvoiceTime { get; set; }
		public string ExpirationTime { get; set; }
		public string CurrentTime { get; set; }
		public decimal BtcPaid { get; set; }
		public decimal Rate { get; set; }
		public bool ExceptionStatus { get; set; }

		public bool IsExpired {
			get {
				var current = Double.Parse (CurrentTime);
				var expires = Double.Parse (ExpirationTime);
				return current >= expires;
			}
		}
	}
}

