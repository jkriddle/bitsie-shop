using Bitsie.Shop.Api;

namespace Bitsie.Shop.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletApi _walletApi;

        public WalletService(IWalletApi walletApi)
        {
            _walletApi = walletApi;
        }

        /// <summary>
        /// Generate new bitcoin address
        /// </summary>
        /// <returns></returns>
        public string CreateAddress()
        {
            return _walletApi.CreateAddress();
        }

        /// <summary>
        /// Retrieve wallet balance in BTC
        /// </summary>
        /// <returns></returns>
        public decimal GetBalance()
        {
            return _walletApi.GetWalletBalance();
        }

        /// <summary>
        /// Retrieve private key for an address
        /// </summary>
        /// <returns></returns>
        public string GetPrivateKey(string address)
        {
            return _walletApi.GetPrivateKey(address);
        }

    }
}
