using Bitsie.Shop.Web.Api.Models;

namespace Bitsie.Shop.Web.Api.Models
{
    public class WalletViewModel : BaseViewModel
    {
        public decimal BtcBalance { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal UsdBalance
        {
            get
            {
                return BtcBalance * MarketPrice;
            }
        }
    }
}