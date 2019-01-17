using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class Order : Entity
    {
        /// <summary>
        /// User account for this transaction
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Order description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Order Number
        /// </summary>
        public virtual string OrderNumber { get; set; }

        /// <summary>
        /// Bitcoin payment address
        /// </summary>
        public virtual string PaymentAddress { get; set; }

        /// <summary>
        /// Date of transaction
        /// </summary>
        public virtual DateTime OrderDate { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public virtual decimal Subtotal { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public virtual decimal Gratuity { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public virtual decimal Total { get; set; }

        /// <summary>
        /// Total price in BTC
        /// </summary>
        public virtual decimal BtcTotal { get; set; }

        /// <summary>
        /// BTC paid so far
        /// </summary>
        public virtual decimal BtcPaid { get; set; }

        /// <summary>
        /// Type of order
        /// </summary>
        public virtual OrderType OrderType { get; set; }

        /// <summary>
        /// Invoice 
        /// </summary>
        public virtual Invoice Invoice { get; set; }

        /// <summary>
        /// Calculated rate based on USD and BTC price.
        /// Reason for this is Bitpay does not provide an accurate
        /// exchange rate when the order total is less than $5 due to
        /// a rounding issue on their side. So we store the Bitcoin exchange
        /// rate at the time of transaction, and calculate this ourselves to be
        /// accurate when displayed to the user (and for calculating remaining balances)
        /// </summary>
        public virtual decimal Rate
        {
            get
            {
                if (BtcTotal == 0) return 0;
                return Total / BtcTotal;
            }
        }

        /// <summary>
        /// Rate provided by exchange
        /// </summary>
        public virtual decimal ExchangeRate { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        public virtual OrderStatus Status { get; set; }

        /// <summary>
        /// Bitcoin payment URL (i.e. "bitcoin:########?amount=######)
        /// </summary>
        public virtual string PaymentUrl {
            get
            {
                return "bitcoin:" + PaymentAddress + "?amount=" + BtcBalance + "&label=" + Description;
            }
        }

        /// <summary>
        /// Balance to be paid (total minus received)
        /// </summary>
        public virtual decimal BtcBalance
        {
            get { return BtcTotal - BtcPaid; }
        }

        /// <summary>
        /// Balance to be paid (total minus received)
        /// </summary>
        public virtual decimal UsdBalance
        {
            get { return BtcBalance * Rate; }
        }

        /// <summary>
        /// Transactions sent to this order
        /// </summary>
        public virtual IList<Transaction> Transactions { get; set; }

        /// <summary>
        /// Payout information
        /// </summary>
        public virtual Payout Payout { get; set; }

        /// <summary>
        /// Product information
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Subscription information
        /// </summary>
        public virtual Subscription Subscription { get; set; }

        /// <summary>
        /// Freshbooks invoice ID
        /// </summary>
        public virtual long? FreshbooksInvoiceId { get; set; }

        /// <summary>
        /// Freshbooks payment ID
        /// </summary>
        public virtual long? FreshbooksPaymentId { get; set; }

        /// <summary>
        /// Order date in EST timezone
        /// </summary>
        public virtual DateTime LocalOrderDate
        {
            get
            {
                DateTime convertedDate = DateTime.SpecifyKind(OrderDate, DateTimeKind.Utc);
                return TimeZoneInfo.ConvertTimeFromUtc(convertedDate, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            }
        }

        /// <summary>
        /// Customer that is making a purchase.
        /// </summary>
        public virtual User Customer { get; set; }
    }
}