using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using NBitcoin;
using System;
using System.Linq;

namespace Bitsie.Shop.Services
{
    public class HdWalletService : IHdWalletService
    {
        private readonly IBitcoinApi _bitcoinApi;
        private readonly IWalletAddressRepository _walletAddressRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ILogService _logService;

        public HdWalletService(IBitcoinApi bitcoinApi, 
            IWalletAddressRepository walletAddressRepository,
            IWalletRepository walletRepository, ILogService logService)
        {
            _bitcoinApi = bitcoinApi;
            _walletAddressRepository = walletAddressRepository;
            _walletRepository = walletRepository;
            _logService = logService;
        }

        public void CreateOrder(Order order)
        {

            var wallet = order.User.Wallet;

            // If the order is for a Tipsie account, and that Tipsie account has a parent merchant, we want
            // the order to be processed by the merchant's payout provider
            if (order.User.Role == Role.Tipsie && order.User.Merchant != null)
            {
                wallet = order.User.Merchant.Wallet;
            }


            DateTime now = DateTime.UtcNow;
            DateTime expiration = DateTime.UtcNow.AddMinutes(15);

            // For more info see WalletRepository comments.
            int nextDerivation = _walletRepository.GetNextDerivation(wallet.Id);

            var address =_walletAddressRepository.FindAll().FirstOrDefault(a => a.Wallet == wallet && a.Derivation == nextDerivation);
            if (address == null)
            {
                // Create a new derived address
                BitcoinExtPubKey b58pubkey = Network.Main.CreateBitcoinExtPubKey(ExtPubKey.Parse(wallet.PublicMasterKey));

                // And try deriving a child
                var customerChildKey = b58pubkey.ExtPubKey.Derive(new KeyPath("0/" + nextDerivation.ToString()));
                var addressKey = Network.Main.CreateBitcoinExtPubKey(customerChildKey);

                address = new WalletAddress
                {
                    Address = addressKey.ExtPubKey.PubKey.GetAddress(Network.Main).ToWif(),
                    DateExpires = expiration,
                    Derivation = nextDerivation,
                    Wallet = wallet,
                    IsUsed = false
                };

                _logService.CreateLog(new Log
                {
                    Category = Domain.LogCategory.Application,
                    Level = LogLevel.Info,
                    LogDate = DateTime.UtcNow,
                    Message = "Create new BIP32 address: " + address.Address
                });

                wallet.LastDerivation = nextDerivation;
                _walletRepository.Save(wallet);
                _walletAddressRepository.Save(address);

                try
                {
                    string id = _bitcoinApi.CreateWebhook(address.Address, 0);

                    _logService.CreateLog(new Log
                    {
                        Category = Domain.LogCategory.Application,
                        Level = LogLevel.Info,
                        LogDate = DateTime.UtcNow,
                        Message = "Created webhook: " + id
                    });

                }
                catch (Exception ex)
                {
                    _logService.CreateLog(new Log
                    {
                        Category = Domain.LogCategory.Application,
                        Level = LogLevel.Info,
                        LogDate = DateTime.UtcNow,
                        Message = "Failed to add webhook for bitcoin address: " + address.Address,
                        Details = ex.Message + "\r\n" + ex.StackTrace
                    });
                    throw;
                }

            }
            else
            {
                // Update expiration so this isn't re-used by another customer until after expiration
                address.DateExpires = expiration;
                _walletAddressRepository.Save(address);
            }

            // Add pricing info
            decimal marketPrice = _bitcoinApi.GetMarketPrice();
            order.BtcTotal = Math.Round(order.Total / marketPrice, 8);
            order.PaymentAddress = address.Address;
            order.ExchangeRate = marketPrice;
            order.OrderNumber = GuidHelper.Create(12);
            order.Status = OrderStatus.Pending;
            order.OrderDate = DateTime.UtcNow;
        }

        public void SaveWallet(Bip39Wallet wallet)
        {
            _walletRepository.Save(wallet);
        }
    }
}
