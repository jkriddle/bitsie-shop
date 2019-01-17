using System;
using System.Web.Mvc;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Models;
using Bitsie.Shop.Web.Providers;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Controllers
{
    [RedirectHttps]
    public class TipsieController : BaseController
    {

        #region Constructor

        public TipsieController(IUserService userService) 
            : base(userService)
        {
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Start()
        {
            return View("SignUpStart");
        }

        [RequiresAuthentication, RequiresRole(Role.Tipsie)]
        public ActionResult Dashboard()
        {
            return View("Dashboard");
        }

        [RequiresAuthentication, RequiresRole(Role.Tipsie)]
        public ActionResult Print()
        {
            return View("Print");
        }

        #endregion

    }
}
