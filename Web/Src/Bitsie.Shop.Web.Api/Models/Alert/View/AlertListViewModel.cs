using System.Collections.Generic;
namespace Bitsie.Shop.Web.Api.Models
{
    public class AlertListViewModel
    {
        public AlertListViewModel()
        {
            Alerts = new List<AlertViewModel>();
        }

        public IList<AlertViewModel> Alerts { get; set; }
    }
}