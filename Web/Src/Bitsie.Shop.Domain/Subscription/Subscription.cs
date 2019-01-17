using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
namespace Bitsie.Shop.Domain
{
    public class Subscription : Entity
    {
        /// <summary>
        /// Date of first subscription
        /// </summary>
        public virtual DateTime DateSubscribed { get; set; }

        /// <summary>
        /// Date of last renewal
        /// </summary>
        public virtual DateTime DateRenewed { get; set; }

        /// <summary>
        /// Expiration date
        /// </summary>
        public virtual DateTime DateExpires { get; set; }

        /// <summary>
        /// Type of subscription
        /// </summary>
        public virtual SubscriptionType Type { get; set; }

        /// <summary>
        /// Price of subscription (for future grandfathered accounts)
        /// </summary>
        public virtual decimal Price { get; set; }

        /// <summary>
        /// History of customer renewals
        /// </summary>
        public virtual IList<Order> Orders { get; set; }

        /// <summary>
        /// Status of subscription
        /// </summary>
        public virtual SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Length of subscription
        /// </summary>
        public virtual SubscriptionTerm Term { get; set; }

        /// <summary>
        /// Reference to user with this subscription
        /// </summary>
        public virtual User User { get; set; }
    }
}
