using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public class CoinbaseOrder
    {
        public string OrderId { get; set; }
        public string ReceiveAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal BtcPrice { get; set; }
        public decimal Price { get; set; }
    }
}
