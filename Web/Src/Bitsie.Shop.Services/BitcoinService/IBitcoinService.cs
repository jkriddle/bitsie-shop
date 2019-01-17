using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public interface IBitcoinService
    {
        decimal GetMarketPrice();
        int GetBlockHeight();
        IList<BitcoinTransaction> GetTransactions(string address);
        BitcoinTransaction GetTransaction(string address, string txId);
        BitcoinAddress GetAddressInfo(string address);
        string CreateWebhook(string address, int confirmations);
    }
}
