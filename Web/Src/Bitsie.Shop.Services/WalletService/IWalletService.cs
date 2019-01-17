using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public interface IWalletService
    {
        string CreateAddress();
        decimal GetBalance();
        string GetPrivateKey(string address);
    }
}
