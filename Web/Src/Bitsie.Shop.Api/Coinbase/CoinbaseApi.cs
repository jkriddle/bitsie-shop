using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class CoinbaseApi : IPaymentProcessorApi
    {
        private CoinbaseConnector _connector;
        private const int SatoshiPerBtc = 100000000;

        public CoinbaseApi(string apiKey, string apiSecret)
        {
            _connector = new CoinbaseConnector(apiKey, apiSecret);
        }

        public decimal GetAccountBalance()
        {
            var js = _connector.GetAccountBalance();
            JObject json = JObject.Parse(_connector.GetAccountBalance());
            return decimal.Parse(json["amount"].ToString());
        }

        public void CreateOrder(Order order, string callbackUrl)
        {
            var js = _connector.CreateOrder("Order for user ID " + order.User.Id, order.Total, callbackUrl);
            JObject json = JObject.Parse(js);

            MapOrderJson(order, json);
            order.Status = OrderStatus.Pending;
        }

        public Order GetOrder(string orderNumber)
        {
            var js = _connector.GetOrder(orderNumber);
            JObject json = JObject.Parse(js);
            var order = new Order();
            MapOrderJson(order, json);
            return order;
        }

        private void MapOrderJson(Order order, JObject json)
        {
            decimal btc = decimal.Parse(json["order"]["total_btc"]["cents"].ToString()) / SatoshiPerBtc;
            decimal usd = decimal.Parse(json["order"]["total_native"]["cents"].ToString()) / 100;

            order.OrderNumber = json["order"]["id"].ToString();
            order.BtcTotal = btc;
            order.PaymentAddress = json["order"]["receive_address"].ToString();
            order.OrderDate = DateTime.UtcNow;
            switch (json["order"]["status"].ToString())
            {
                case "new":
                    order.Status = OrderStatus.Pending;
                    break;
                case "completed":
                    order.Status = OrderStatus.Complete;
                    break;
                case "cancelled":
                    order.Status = OrderStatus.Expired;
                    break;
                case "mispaid":
                    order.Status = OrderStatus.Partial;
                    break;
                case "expired":
                    order.Status = OrderStatus.Expired;
                    break;
            }
            order.ExchangeRate = ((decimal)usd / btc);
            order.Total = usd;
        }
    }
}