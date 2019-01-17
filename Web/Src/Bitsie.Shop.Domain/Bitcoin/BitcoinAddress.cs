using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class BitcoinAddress
    {
        public BitcoinAddress()
        {
            Transactions = new List<BitcoinTransaction>();
        }

        public string Address { get; set; }
        public decimal Balance { get; set; }
        public IList<BitcoinTransaction> Transactions { get; set; }
    }
}
