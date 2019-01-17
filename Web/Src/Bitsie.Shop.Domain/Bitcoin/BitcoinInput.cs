using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class BitcoinInput
    {
        public string Address { get; set; }
        public long Value { get; set; }

        public BitcoinInput(string address, long value)
        {
            Address = address;
            Value = value;
        }
    }
}
