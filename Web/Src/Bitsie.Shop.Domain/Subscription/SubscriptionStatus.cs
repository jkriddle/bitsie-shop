using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum SubscriptionStatus
    {
        /// <summary>
        /// Paid and current
        /// </summary>
        [Description("Active")]
        Active = 1,

        /// <summary>
        /// Not paid for yet
        /// </summary>
        [Description("Pending")]
        Pending,

        /// <summary>
        /// Expired and passed probationary period
        /// </summary>
        [Description("Expired")]
        Expired,

        /// <summary>
        /// Manually cancelled
        /// </summary>
        [Description("Cancelled")]
        Cancelled,

        /// <summary>
        /// Subscription has expired but user has X days to pay
        /// </summary>
        //[Description("Probation")]
        //Probation
        //TODO - do this later?
    }
}
