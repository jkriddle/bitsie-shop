using System.Web;
using System.Web.Http;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Domain;
using System;
using Bitsie.Shop.Api;
using System.Linq;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class WalletController : BaseApiController
    {
        #region Fields

        private readonly IMapperService _mapper;
        private readonly IOrderService _orderService;
        private readonly IBitcoinService _bitcoinService;
        private readonly IWalletService _walletService;
        private readonly IConfigService _configService;

        #endregion

        #region Constructor

        public WalletController(IUserService userService,
            ILogService logService,
            IOrderService orderService,
            IBitcoinService bitcoinService,
            IWalletService walletService,
            IConfigService configService,
            IMapperService mapper)
            : base(userService, logService)
        {
            _mapper = mapper;
            _orderService = orderService;
            _bitcoinService = bitcoinService;
            _configService = configService;
            _walletService = walletService;
        }

        #endregion

        #region Public Methods

        [HttpGet, NoCache, ApiRequiresRole(Role.Administrator)]
        public WalletViewModel Balance(OrderInputModel inputModel)
        {
            var vm = new WalletViewModel();
            vm.BtcBalance = _walletService.GetBalance();
            vm.MarketPrice = _bitcoinService.GetMarketPrice();
            return vm;
        }


        [HttpGet, NoCache, RequiresApiAuth]
        public string GetPrivateKey(string address)
        {
            var offline = CurrentUser.OfflineAddresses.FirstOrDefault(a => a.Address == address 
                && a.EncryptedPrivateKey != null);

            if (offline == null)
            {
                throw new HttpException(404, "Address not found.");
            }


            var kp = offline.GetKeyPair(System.Text.Encoding.Default.GetString(CurrentUser.HashedPassword));
            return kp.PrivateKeyBase58;
        }

       
        #endregion


    }

}
