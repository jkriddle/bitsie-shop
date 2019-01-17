using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Api
{
    public interface IWalletApi
    {
        string CreateAddress(string description = null);
        string SendMany(IList<BitcoinPayment> payouts);
        string Send(BitcoinPayment payment);
        decimal GetWalletBalance();
        Order GetOrder(string orderNumber);
        void CreateOrder(Order order);
        string GetPrivateKey(string address);
    }
}
