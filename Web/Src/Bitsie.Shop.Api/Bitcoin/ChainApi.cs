using Bitsie.Shop.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class ChainApi : BaseApi, IBitcoinApi
    {
        public static int SATOSHI_PER_BTC = 100000000;
        NetworkCredential _credentials;
        private string _webhook;

        public ChainApi(string apiKey, string apiSecret, string webhook) 
            : base("https://" + apiKey + ":" + apiSecret + "@api.chain.com/v2/")
        {
            _credentials = new NetworkCredential(apiKey, apiSecret);
            _webhook = webhook;
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
                throw new InvalidOperationException("Unable to parse JSON from blockchain API");
            }
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


        public string CreateWebhook(string address, int confirmations)
        {
            var nv = new JObject();
            nv.Add("type", "address");
            nv.Add("block_chain", "bitcoin");
            nv.Add("address", address);
            nv.Add("url", "https://shop.bitsie.com/IPN/Update");
            string content = SendRequest(GetUrl("notifications"), "POST", nv, _credentials);
            CheckForError(content);
            var json = JObject.Parse(content);
            return json["id"].ToString();
        }

        public int GetBlockHeight()
        {
            var content = SendRequest("bitcoin/blocks/latest", "GET", null, _credentials);
            CheckForError(content);
            var json = JObject.Parse(content);
            return int.Parse(json["height"].ToString());
        }

        public IList<BitcoinTransaction> GetTransactions(string address)
        {
            var transactions = new List<BitcoinTransaction>();
            var content = SendRequest("bitcoin/addresses/" + address + "/transactions", "GET", null, _credentials);

            JArray json = JArray.Parse(content);

            if (json.Count > 0)
            {
                foreach (var t in json)
                {

                    long amount = 0;
                    foreach (var o in t["outputs"])
                    {
                        foreach (var a in o["addresses"])
                        {
                            if (a.ToString() == address)
                            {
                                amount = o["value"].Value<long>();
                            }
                        }
                    }

                    var tran = new BitcoinTransaction
                    {
                        Address = address,
                        Block = t["block_height"].Value<int?>(),
                        Category = TransactionCategory.Receive,
                        TransactionDate = t["block_time"].Value<DateTime?>().HasValue ? t["block_time"].Value<DateTime>() : DateTime.UtcNow,
                        TxId = t["hash"].ToString(),
                        Value = amount / (decimal)SATOSHI_PER_BTC
                    };

                    foreach (var o in t["inputs"])
                    {
                        foreach (var a in o["addresses"])
                        {
                            tran.Inputs.Add(new BitcoinInput(a.ToString(), long.Parse(o["value"].ToString())));
                        }
                    }

                    transactions.Add(tran);
                }
            }
            return transactions;
        }

        public BitcoinTransaction GetTransaction(string address, string txId)
        {
            var content = SendRequest("bitcoin/transactions/" + txId, "GET", null, _credentials);
            CheckForError(content);
            JObject json = JObject.Parse(content);

            int confirmations = int.Parse(json["confirmations"].ToString());

            long amount = 0;
            foreach (var o in json["outputs"])
            {
                foreach (var a in o["addresses"])
                {
                    if (a.ToString() == address)
                    {
                        amount = o["value"].Value<long>();
                    }
                }
            }

            var tran = new BitcoinTransaction
            {
                TransactionDate = json["chain_received_at"].Value<DateTime>(),
                TxId = txId,
                Block = json["block_height"].Value<int?>(),
                Value = amount / (decimal)SATOSHI_PER_BTC,
                Category = TransactionCategory.Receive
            };

            foreach (var o in json["inputs"])
            {
                foreach (var a in o["addresses"])
                {
                    tran.Inputs.Add(new BitcoinInput(a.ToString(), long.Parse(o["value"].ToString())));
                }
            }

            return tran;
        }

        public BitcoinAddress GetAddress(string address)
        {
            var content = SendRequest("bitcoin/addresses/" + address, "GET", null, _credentials);
            CheckForError(content);
            JObject json = JObject.Parse(content);

            // Get balance
            var addr = new BitcoinAddress
            {
                Address = address,
                Balance = long.Parse(json["balance"].ToString()) / SATOSHI_PER_BTC
            };

            return addr;
        }

        public decimal GetMarketPrice()
        {
            var content = base.SendRequest("https://www.bitstamp.net/api/ticker/", "GET");
            var json = JObject.Parse(content);
            return decimal.Parse(json["last"].ToString());
        }      

    }
}
