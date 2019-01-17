using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum OrderType
    {
        [Description("Point of Sale")]
        PointOfSale,
        [Description("Hosted Checkout")]
        HostedCheckout,
        [Description("Offline Address")]
        OfflineAddress,
        [Description("Subscription")]
        Subscription
    }
}
