using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Providers;
using System.Text.RegularExpressions;
using System.Linq;
using Bitsie.Shop.Api;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class IntegrationsController : BaseApiController
    {
        #region Fields

        private readonly IHdWalletService _hdWalletService;

        #endregion

        #region Constructor

        public IntegrationsController(IUserService userService, 
            ILogService logService, IHdWalletService hdWalletService) : base(userService, logService)
        {
            _hdWalletService = hdWalletService;
        }

        #endregion

        #region Public Methods

        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Bitcoin(BitcoinIntegrationInputModel inputModel)
        {
            var vm = new BaseResponseModel();
            
            var validationState = new ValidationDictionary();
            if (inputModel.ValidateRequest(validationState))
            {
                ClearIntegrations();
                CurrentUser.Settings.PaymentMethod = PaymentMethod.Bitcoin;
                CurrentUser.Settings.PaymentAddress = inputModel.PaymentAddress;
                UserService.UpdateUser(CurrentUser);
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;

            return vm;
        }

        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Bip44(Bip44IntegrationInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            var validationState = new ValidationDictionary();
            if (inputModel.ValidateRequest(validationState))
            {
                ClearIntegrations();

                switch (inputModel.walletName)
                {
                    case "Trezor":
                        CurrentUser.Settings.PaymentMethod = PaymentMethod.Trezor;
                        break;
                    case "Wallet32":
                        CurrentUser.Settings.PaymentMethod = PaymentMethod.Wallet32;
                        break;
                }

                if (CurrentUser.Wallet == null)
                {
                    CurrentUser.Wallet = new Bip39Wallet
                    {
                        User = CurrentUser,
                    };
                }

                if (CurrentUser.Wallet.PublicMasterKey != inputModel.MasterPublicKey)
                {
                    // User is changing wallet, reset to use first derivation
                    CurrentUser.Wallet.LastDerivation = null;
                }

                CurrentUser.Wallet.PublicMasterKey = inputModel.MasterPublicKey;
                CurrentUser.Wallet.EncryptedPrivateMasterKey = inputModel.EncryptedPrivateKey;
                _hdWalletService.SaveWallet(CurrentUser.Wallet);
                UserService.UpdateUser(CurrentUser);
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;

            return vm;
        }

        [HttpPost, RequiresApiAuth]
        public string GetPrivateKeyHash()
        {
            return CurrentUser.Wallet.EncryptedPrivateMasterKey;
        }

        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Bitpay(BitpayIntegrationInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            var validationState = new ValidationDictionary();
            if (inputModel.ValidateRequest(validationState))
            {
                // Test account
                var bitpayApi = new BitpayApi(inputModel.BitpayApiKey);
                try
                {
                    bitpayApi.CreateOrder(new Order
                    {
                        User = CurrentUser,
                        Total = 5.00m,
                        OrderDate = DateTime.UtcNow,
                        Subtotal = 5.00m
                    }, "");
                }
                catch (Exception)
                {
                    vm.Success = false;
                    vm.Errors.Add("Invalid API key, please check your key and try again.");
                    return vm;
                }

                ClearIntegrations();
                CurrentUser.Settings.PaymentMethod = PaymentMethod.Bitpay;
                CurrentUser.Settings.BitpayApiKey = inputModel.BitpayApiKey;
                UserService.UpdateUser(CurrentUser);
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;

            return vm;
        }

        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Coinbase(CoinbaseIntegrationInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            var validationState = new ValidationDictionary();
            if (inputModel.ValidateRequest(validationState))
            {
                // Test account
                var coinbaseApi = new CoinbaseApi(inputModel.CoinbaseApiKey, inputModel.CoinbaseApiSecret);
                try
                {
                    coinbaseApi.CreateOrder(new Order
                    {
                        User = CurrentUser,
                        Total = 5.00m,
                        OrderDate = DateTime.UtcNow,
                        Subtotal = 5.00m
                    }, "");
                }
                catch (Exception)
                {
                    vm.Success = false;
                    vm.Errors.Add("Invalid API key, please check your key and try again.");
                    return vm;
                }

                ClearIntegrations();
                CurrentUser.Settings.PaymentMethod = PaymentMethod.Coinbase;
                CurrentUser.Settings.CoinbaseApiKey = inputModel.CoinbaseApiKey;
                CurrentUser.Settings.CoinbaseApiSecret = inputModel.CoinbaseApiSecret;
                UserService.UpdateUser(CurrentUser);
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;

            return vm;
        }

        [HttpPost, RequiresApiAuth]
        public BaseResponseModel DisableFreshbooks()
        {
            var vm = new BaseResponseModel();
            CurrentUser.Settings.FreshbooksApiUrl = null;
            CurrentUser.Settings.FreshbooksAuthToken = null;
            UserService.UpdateUser(CurrentUser);
            return vm;
        }

        private void ClearIntegrations()
        {
            CurrentUser.Settings.PaymentAddress = 
                CurrentUser.Settings.BitpayApiKey = 
                CurrentUser.Settings.CoinbaseApiKey = 
                CurrentUser.Settings.CoinbaseApiSecret = String.Empty;
        }

        #endregion
    }

}
