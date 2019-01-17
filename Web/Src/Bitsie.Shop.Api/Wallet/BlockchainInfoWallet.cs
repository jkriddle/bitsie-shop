using Bitsie.Shop.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class BlockchainWalletApi : BaseApi, IWalletApi
    {
        public static int SATOSHI_PER_BTC = 100000000;
        protected readonly string Guid;
        protected readonly string Password;
        protected readonly string SecondPassword;

        public BlockchainWalletApi(string guid, string password, string secondPassword)
            : base("https://blockchain.info/")
        {
            Guid = guid;
            Password = password;
            SecondPassword = secondPassword;
        }

        public Order GetOrder(string orderNumber)
        {
            throw new NotImplementedException("Blockchain.info does not retrieve orders by label.");
        }

        /// <summary>
        /// Send payments to bitcoin addresses
        /// </summary>
        /// <param name="payouts"></param>
        /// <returns>Transaction hash</returns>
        public string SendMany(IList<BitcoinPayment> payments)
        {
            JObject recipients = new JObject();
            foreach (var p in payments)
            {
                recipients.Add(new JProperty(p.Address, (long)(p.Amount * SATOSHI_PER_BTC)));
            }
            string url = "merchant/" + Guid + "/sendmany?recipients=" + recipients.ToString();
            var js = SendRequest(GetUrl(url), "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            return json["tx_hash"].ToString();
        }

        public string Send(BitcoinPayment payment)
        {
            long amount = Convert.ToInt64(payment.Amount * SATOSHI_PER_BTC);
            string url = "merchant/" + Guid + "/payment?to=" + payment.Address + "&amount=" + amount;
            var js = SendRequest(GetUrl(url), "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            return json["tx_hash"].ToString();
        }

        public void CreateOrder(Order order)
        {
            string url = "merchant/" + Guid + "/new_address";
            if (!String.IsNullOrEmpty(order.Description)) url += "?label=" + order.Description;
            var js = SendRequest(GetUrl(url), "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            string address = json["address"].ToString();

            decimal marketPrice = GetMarketPrice();
            // some wallets don't support past 7 decimals (blockchain.info) so round it off
            order.BtcTotal = Math.Round(order.Total / marketPrice, 8);
            order.PaymentAddress = address;
            order.ExchangeRate = marketPrice;
            order.OrderNumber = GuidHelper.Create(12);
            order.Status = OrderStatus.Pending;
            order.OrderDate = DateTime.UtcNow;
        }

        protected new string GetUrl(string path)
        {
            string fullPath = path;
            if (fullPath.Contains("?")) fullPath += "&";
            else fullPath += "?";
            fullPath += "password=" + Password + "&second_password=" + SecondPassword;
            return RootUrl + fullPath;
        }

        protected decimal GetMarketPrice()
        {
            var js = SendRequest("ticker", "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            return decimal.Parse(json["USD"]["15m"].ToString());
        }

        public string CreateAddress(string label)
        {
            string url = "merchant/" + Guid + "/new_address";
            var js = SendRequest(GetUrl(url), "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            return json["address"].ToString();
        }

        public decimal GetWalletBalance()
        {
            string url = "merchant/" + Guid + "/balance";
            var js = SendRequest(GetUrl(url), "GET");
            CheckForError(js);
            JObject json = JObject.Parse(js);
            return decimal.Parse(json["balance"].ToString()) / SATOSHI_PER_BTC;
        }

        public string GetPrivateKey(string address)
        {
            throw new NotImplementedException("Not implemented...how to do this?");
        }

        protected void CheckForError(string content)
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
                throw new InvalidOperationException(json["error"].ToString());
            }
        }
    }
}