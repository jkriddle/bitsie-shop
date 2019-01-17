using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum SubscriptionType
    {
        [Description("Basic")]
        Basic = 1,
        [Description("Premium")]
        Premium,
        [Description("Unlimited")]
        Unlimited
    }
}
