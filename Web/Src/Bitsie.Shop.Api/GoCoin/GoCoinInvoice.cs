using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class GoCoinInvoice
    {
        public string Id { get; set; }

        public string Status { get; set; }

        [JsonProperty(PropertyName = "payment_address")]
        public string PaymentAddress { get; set; }

        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "crypto_balance_due")]
        public decimal CryptoBalanceDue { get; set; }

        [JsonProperty(PropertyName = "price_currency")]
        public string PriceCurrency { get; set; }

        [JsonProperty(PropertyName = "base_price")]
        public decimal BasePrice { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal BtcPrice { get; set; }

        [JsonProperty(PropertyName = "created_at"), JsonConverter(typeof(EpochDateTimeConverter))]
        public DateTime InvoiceTime { get; set; }

        [JsonProperty(PropertyName = "inverse_spot_rate")]
        public decimal Rate { get; set; }

       /*
      "id": "84c4fc04-66f2-49a5-a12a-36baf7f9f450",
    "status": "unpaid",
    "payment_address": "1xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "price": "1.00000000",
    "crypto_balance_due": "1.00000000",
    "price_currency": "BTC",
    "valid_bill_payment_currencies": null,
    "base_price": "134.00",
    "base_price_currency": "USD",
    "service_fee_rate": "0.01",
    "usd_spot_rate": "1.0",
    "spot_rate": "0.00746268656716",
    "inverse_spot_rate": "134.0",
    "crypto_payout_split": "100",
    "confirmations_required": 2,
    "crypto_url": null,
    "gateway_url": "https://gateway.gocoin.com/invoices/84c4fc04-66f2-49a5-a12a-36baf7f9f450",
    "notification_level": null,
    "redirect_url": "http://www.example.com/redirect",
    "order_id": null,
    "item_name": null,
    "item_sku": null,
    "item_description": null,
    "physical": null,
    "customer_name": null,
    "customer_address_1": null,
    "customer_address_2": null,
    "customer_city": null,
    "customer_region": null,
    "customer_country": null,
    "customer_postal_code": null,
    "customer_email": null,
    "customer_phone": null,
    "user_defined_1": null,
    "user_defined_2": null,
    "user_defined_3": null,
    "user_defined_4": null,
    "user_defined_5": null,
    "user_defined_6": null,
    "user_defined_7": null,
    "user_defined_8": null,
    "data": null,
    "expires_at": "2014-01-02T22:08:09.599Z",
    "created_at": "2014-01-02T21:53:10.867Z",
    "updated_at": "2014-01-02T21:53:10.867Z",
    "server_time": "2014-01-02T23:59:12Z",
    "callback_url": "https://www.example.com/gocoin/callback",
    "merchant_id": "7af834d9-aa7a-423c-be16-33ea6a724007"*/
    }
}
