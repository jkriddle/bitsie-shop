using System.Collections.Generic;
using Bitsie.Shop.Services;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OfflineAddressListViewModel
    {
        #region Constructor

        public OfflineAddressListViewModel(IList<OfflineAddress> addresses)
        {
            OfflineAddresses = new List<OfflineAddressViewModel>();
            foreach (OfflineAddress address in addresses)
            {
                OfflineAddresses.Add(new OfflineAddressViewModel(address));
            }
        }

        #endregion

        #region Public Properties

        public IList<OfflineAddressViewModel> OfflineAddresses { get; set; }

        #endregion
    }
}