using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum SubscriptionTerm
    {
        [Description("Monthly")]
        Monthly = 1,
        [Description("Yearly")]
        Yearly
    }
}
