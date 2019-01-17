using Bitsie.Shop.Domain;
using System.Collections.Generic;

namespace Bitsie.Shop.Api
{
    public interface IBitcoinApi
    {
        decimal GetMarketPrice();
        int GetBlockHeight();
        BitcoinTransaction GetTransaction(string address, string txId);
        IList<BitcoinTransaction> GetTransactions(string address);
        BitcoinAddress GetAddress(string address);
        string CreateWebhook(string address, int confirmations);
    }
}
