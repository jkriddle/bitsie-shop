using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class BitcoinTransaction
    {
        public string Address { get; set; }
        public string TxId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Value { get; set; }
        public int? Block { get; set; }
        public TransactionCategory Category { get; set; }
        public IList<BitcoinInput> Inputs { get; set; }

        public BitcoinTransaction()
        {
            Inputs = new List<BitcoinInput>();
        }
    }
}
