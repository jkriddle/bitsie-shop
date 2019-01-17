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
using System.Collections.Generic;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class AlertController : BaseApiController
    {
        private readonly IConfigService _configService;

        public AlertController(IUserService userService,
            ILogService logService)
            : base(userService, logService)
        {
        }

        [HttpGet, NoCache, RequiresApiAuth]
        public AlertListViewModel Get()
        {
            var vm = new AlertListViewModel();

            //TODO: move to service layer
            if (!CurrentUser.Settings.PaymentMethod.HasValue)
            {
                vm.Alerts.Add(new AlertViewModel(1000, "Settlement method has not been selected for this account."));
            }

            return vm;
        }

    }
}
