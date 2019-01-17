using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class ProcessPayoutResult
    {
        /// <summary>
        /// Bitcoin transaction ID if applicable
        /// </summary>
        public string TxId { get; set; }
    }
}
