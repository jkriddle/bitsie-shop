using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class CustomerController : BaseMerchantController
    {
        private readonly IUserService _userService;
        private readonly IConfigService _configService;

        public CustomerController(IUserService userService, 
            IConfigService configService) : base(userService)
        {
            _userService = userService;
            _configService = configService;
        }

        /// <summary>
        /// New customer signup
        /// </summary>
        /// <param name="merchantId"></param>
        /// <returns></returns>
        public ActionResult Signup(string id)
        {
            var merchant = GetMerchant(id);
            var vm = new BaseMerchantViewModel
            {
                Merchant = merchant
            };
            return MerchantView(merchant, "Signup", vm);
        }

    }
}