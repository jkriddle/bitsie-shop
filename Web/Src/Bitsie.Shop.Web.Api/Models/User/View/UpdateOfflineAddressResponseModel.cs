using Bitsie.Shop.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UpdateOfflineAddressResponseModel : BaseResponseModel
    {
        public IList<OfflineAddressViewModel> OfflineAddresses { get; set; }

        public UpdateOfflineAddressResponseModel(IList<OfflineAddress> addresses)
        {
            OfflineAddresses = new List<OfflineAddressViewModel>();
            foreach (OfflineAddress address in addresses)
            {
                OfflineAddresses.Add(new OfflineAddressViewModel(address));
            }
        }

    }
}