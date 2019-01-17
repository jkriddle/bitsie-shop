using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Mobile.Controllers
{
    public class AppController : BaseController
    {
        public AppController(IUserService userService) : base(userService)
        {
        }

        public ActionResult Index()
        {
            return View("SignIn");
        }

        public ActionResult AmountEntry()
        {
            return View("AmountEntry", CurrentUser);
        }

        public ActionResult Pay()
        {
            return View("Pay");
        }
    }
}
