using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public enum PaymentMethod
    {
        [Description("Bitpay")]
        Bitpay = 1,
        [Description("Bitcoin")]
        Bitcoin,
        [Description("Coinbase")]
        Coinbase,
        [Description("GoCoin")]
        GoCoin,
        [Description("Trezor")]
        Trezor,
        [Description("Wallet 32")]
        Wallet32
    }
}
