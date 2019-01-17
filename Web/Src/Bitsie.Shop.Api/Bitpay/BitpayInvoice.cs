using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitsie.Shop.Api
{
	public class BitpayInvoice
	{
		public string Id { get; set; }
		public string Url { get; set; }
		public string Status { get; set; }
		public decimal BtcPrice { get; set; }
		public decimal Price { get; set; }
        public string Currency { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime InvoiceTime { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime ExpirationTime { get; set; }
        [JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime CurrentTime { get; set; }

		public decimal BtcPaid { get; set; }
		public decimal Rate { get; set; }
		public bool ExceptionStatus { get; set; }
	}
}

