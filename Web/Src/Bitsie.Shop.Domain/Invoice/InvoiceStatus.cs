using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public enum InvoiceStatus
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
        // Order expired before fully paid
        [Description("Expired")]
        Expired,
    }
}
