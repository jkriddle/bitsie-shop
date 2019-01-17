using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class BitcoinPayment
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
    }
}
