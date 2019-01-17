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
using GoCoinAPI;

namespace Bitsie.Shop.Api
{
    public class GoCoinApi : IPaymentProcessorApi
    {
        private string _apiKey;
        private string _apiSecret;

        public GoCoinApi(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
		}

        private Client _client;
        private Client Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new Client();
                    _client.client_id = _apiKey;
                    _client.client_secret = _apiSecret;
                    _client.scope = "user_read_write merchant_read_write invoice_read_write oauth_read_write";
                }
                return _client;
            }
        }

        public string Authenticate(string redirectUrl)
        {
            Client.redirect_uri = redirectUrl;
            string fullRedirect = Client.request_client(Client.secure) + "://" + Client.get_auth_url(redirectUrl);
            Client.initToken();
            Boolean b_auth = Client.authorize_api();
            return fullRedirect;
        }

        public string GetAccessToken(string authcode, string redirectUrl)
        {
            AuthorizationCode code = new AuthorizationCode();
            code.client_id = _apiKey;
            code.client_secret = _apiSecret;
            code.code = authcode;
            code.grant_type = "authorization_code";
            code.redirect_uri = redirectUrl;
            var result = new AccessToken();
            var auth = new Auth(Client);
            var authResult = auth.authenticate(code);
            return authResult.access_token;
        }

        public Order GetOrder(string orderNumber)
        {
            GoCoinAPI.User currentuser = new GoCoinAPI.User();
            currentuser = Client.api.user.self();

            var invoice = Client.api.invoices.get(orderNumber);

            var order = new Order();
            order.OrderNumber = invoice.id;
            order.BtcTotal = (decimal)invoice.price;
            order.OrderDate = DateTime.UtcNow;
            order.PaymentAddress = invoice.payment_address;
            order.ExchangeRate = (decimal)invoice.btc_spot_rate;
            
            switch (invoice.status)
            {
                case "unpaid":
                    order.Status = OrderStatus.Pending;
                    break;
                case "paid":
                    order.Status = OrderStatus.Paid;
                    break;
                case "ready_to_ship":
                    order.Status = OrderStatus.Confirmed;
                    break;
                case "fulfilled":
                    order.Status = OrderStatus.Complete;
                    break;
                case "merchant_review":
                case "underpaid":
                    order.Status = OrderStatus.Partial;
                    break;
                default:
                    order.Status = OrderStatus.Pending;
                    break;
            }

            //TODO: take json expires_at date and determine expiration
            DateTime expirationDate = order.OrderDate.AddMinutes(15);
            //Note: an invoice never passes into an 'Expired' state - the expiration window is based on the value of "expires_at" in the Invoice.
            if (order.Status == OrderStatus.Pending && DateTime.UtcNow > expirationDate)
            {
                order.Status = OrderStatus.Expired;
            }

            order.Total = (decimal)invoice.base_price;
            return order;
		}

        public void SetAccessToken(string token)
        {
            Client.token = token;
        }

		public void CreateOrder(Order order, string callbackUrl) {
            GoCoinAPI.User currentuser = new GoCoinAPI.User();
            currentuser = Client.api.user.self();

            GoCoinAPI.Invoices invoices = new GoCoinAPI.Invoices();
            invoices.price_currency = "BTC";
            invoices.base_price = (float)order.Total;
            invoices.callback_url = callbackUrl;
            invoices.base_price_currency = "USD";
            invoices.confirmations_required = 0;
            invoices.notification_level = "all";

            var response = Client.api.invoices.create(currentuser.merchant_id, invoices);
            order.ExchangeRate = response.inverse_spot_rate;
            order.OrderNumber = response.id;
            order.PaymentAddress = response.payment_address;
            order.BtcTotal = (decimal)response.price;
            order.Status = OrderStatus.Pending;
		}


	}

}

