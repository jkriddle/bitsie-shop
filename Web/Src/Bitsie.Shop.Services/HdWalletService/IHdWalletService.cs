using Bitsie.Shop.Domain;
namespace Bitsie.Shop.Services
{
    public interface IHdWalletService
    {
        void CreateOrder(Order order);
        void SaveWallet(Bip39Wallet wallet);
    }
}
