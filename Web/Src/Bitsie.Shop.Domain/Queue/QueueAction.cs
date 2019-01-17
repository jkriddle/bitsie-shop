using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum QueueAction
    {
        [Description("Chain IPN")]
        ChainIpn = 1,
        [Description("Bitpay IPN")]
        BitpayIpn,
        [Description("Coinbase IPN")]
        CoinbaseIpn,
        [Description("GoCoin IPN")]
        GoCoinIpn,
        [Description("Cron Request")]
        Cron,
        [Description("Cron Subscriptions Request")]
        Subscriptions
    }
}
