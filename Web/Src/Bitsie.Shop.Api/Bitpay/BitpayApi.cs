using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Bitsie.Shop.Domain;
using Newtonsoft.Json.Converters;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Bitsie.Shop.Api
{
    public class BitpayApi : BaseApi, IPaymentProcessorApi
    {

		private NetworkCredential credentials;

		public BitpayApi(string apiKey) : base("https://bitpay.com/api/") {
            this.credentials = new NetworkCredential(apiKey, "");
		}

		private string GetPaymentUrl(string url) {
			HttpWebRequest request = CreateRequest(url, "GET", credentials, null);
			request.Accept = "text/uri­lis";
            string content = ExecuteRequest(request);
            CheckForError(content);
			Regex reg = new Regex("bitcoin:([^\\\"]+)");
			var matches = reg.Matches (content);
			if (matches.Count == 0)
				throw new Exception("Could not find Bitcoin QR code in merchant service response.");
			return matches[0].Value;
		}

		public Order GetOrder(string orderNumber) {
            string content = SendRequest("invoice/" + orderNumber, "GET", null, credentials);
            CheckForError(content);

            var order = new Order();
            BitpayInvoice invoice = JsonConvert.DeserializeObject<BitpayInvoice>(content);
            MapInvoiceToOrder(order, invoice);
            return order;
		}



        protected string SendRequest(string path, string method = "GET",
                                    JObject data = null,
                                    NetworkCredential credentials = null)
        {
            HttpWebRequest request = CreateRequest(path, method, credentials, null);
            string postData = "";
            if (data != null)
            {
                postData = JsonConvert.SerializeObject(data);
            }

            if (!String.IsNullOrEmpty(postData))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            return ExecuteRequest(request);
        }


		public void CreateOrder(Order order, string callbackUrl) {
			var nv = new JObject();
			nv.Add ("price", order.Total.ToString("N2"));
            nv.Add("currency", "USD");
            nv.Add("notificationURL", callbackUrl);
            nv.Add("fullNotifications", true);
            nv.Add("notificationEmail", "support@bitsie.com");
			string content = SendRequest ("invoice", "POST", nv, credentials);
            CheckForError(content);
            BitpayInvoice invoice = JsonConvert.DeserializeObject<BitpayInvoice>(content);
            MapInvoiceToOrder(order, invoice);
		}

        private void CheckForError(string content)
        {
            JObject json = null;
            try
            {
                json = JObject.Parse(content);
            }
            catch
            {
                // Fail silently
            }

            if (json != null && json["error"] != null)
            {
                throw new InvalidOperationException(json["error"]["message"].ToString());
            }
        }

        private void MapInvoiceToOrder(Order order, BitpayInvoice invoice)
        {
            order.OrderNumber = invoice.Id;
            order.BtcTotal = invoice.BtcPrice;
            order.OrderDate = invoice.InvoiceTime;
            string paymentUrl = GetPaymentUrl(invoice.Url);
            Regex reg = new Regex("^bitcoin:([0-9A-Za-z]+^?)");
            var match = reg.Match(paymentUrl);
            order.PaymentAddress = match.Groups[1].Value;
            order.ExchangeRate = invoice.Rate;
            switch (invoice.Status)
            {
                case "new": 
                    order.Status = OrderStatus.Pending;
                    break;
                case "paid":
                    order.Status = OrderStatus.Paid;
                    break;
                case "confirmed":
                    order.Status = OrderStatus.Confirmed;
                    break;
                case "complete":
                    order.Status = OrderStatus.Complete;
                    break;
                case "expired":
                    order.Status = OrderStatus.Expired;
                    break;
                case "invalid":
                    order.Status = OrderStatus.Partial;
                    break;
            }
            order.Total = invoice.Price;
        }

	}

}

