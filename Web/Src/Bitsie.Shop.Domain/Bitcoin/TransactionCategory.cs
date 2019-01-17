using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public enum TransactionCategory
    {
        [Description("Receive")]
        Receive = 1,
        [Description("Send")]
        Send
    }
}
