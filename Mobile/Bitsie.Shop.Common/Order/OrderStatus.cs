using System;
using System.ComponentModel;

namespace Bitsie.Shop.Common
{
	public enum OrderStatus
	{
		// Order progress goes:
		// Pending > Partial > Paid > Confirmed > Complete

		// Pending = unpaid
		[Description("Pending")]
		Pending = 1,
		// Partial = partial payment sent but not full amount
		[Description("Partial")]
		Partial,
		// Paid = full amount received
		[Description("Paid")]
		Paid,
		// Complete = paid, confirmed and payout sent to merchant
		[Description("Complete")]
		Complete,
		// Order expired before fully paid
		[Description("Expired")]
		Expired,
		// Refund sent to customer
		[Description("Refunded")]
		Refunded,
		// Payment transaction confirmed on blockchain
		[Description("Confirmed")]
		Confirmed,
	}
}

