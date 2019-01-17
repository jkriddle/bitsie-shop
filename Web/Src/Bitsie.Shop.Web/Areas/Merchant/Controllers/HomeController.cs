using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Merchant.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserService userService) : base(userService) { }

        public ActionResult Index()
        {
            return View();
        }
    }
}
