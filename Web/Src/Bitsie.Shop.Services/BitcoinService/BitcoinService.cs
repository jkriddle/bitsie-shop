using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public class BitcoinService : IBitcoinService
    {
        private readonly IBitcoinApi _bitcoinApi;
        private readonly ITransactionRepository _transactionRepository;

        public BitcoinService(IBitcoinApi bitcoinApi, 
            ITransactionRepository transactionRepository)
        {
            _bitcoinApi = bitcoinApi;
            _transactionRepository = transactionRepository;
        }

        public int GetBlockHeight()
        {
            return _bitcoinApi.GetBlockHeight();
        }

        public string CreateWebhook(string address, int confirmations)
        {
            return _bitcoinApi.CreateWebhook(address, confirmations);
        }

        public IList<BitcoinTransaction> GetTransactions(string address)
        {
            return _bitcoinApi.GetTransactions(address);
        }

        public BitcoinTransaction GetTransaction(string address, string txId)
        {
            return _bitcoinApi.GetTransaction(address, txId);
        }

        public decimal GetMarketPrice()
        {
            return _bitcoinApi.GetMarketPrice();
        }

        public BitcoinAddress GetAddressInfo(string address)
        {
            return _bitcoinApi.GetAddress(address);
        }
    }
}
