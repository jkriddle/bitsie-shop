using System;
using System.Web.Mvc;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Models;
using Bitsie.Shop.Web.Providers;

namespace Bitsie.Shop.Web.Controllers
{
    [RedirectHttps]
    public class UserController : BaseController
    {

        #region Fields

        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public UserController(IUserService userService) 
            : base(userService)
        {
            _userService = userService;
        }

        #endregion

        #region Actions

        public ActionResult SignIn(string message = "")
        {
            if (!String.IsNullOrWhiteSpace(message))
            {
                ViewBag.Message = message;
            }
            return View("SignIn");
        }

        public ActionResult SignUp()
        {
            return RedirectToAction("Start");
        }

        public ActionResult Start()
        {
            return View("SignUp/SignUpStart");
        }

        public ActionResult Contact()
        {
            return View("SignUp/SignUpContact");
        }

        public new ActionResult Profile()
        {
            return View("SignUp/SignUpProfile");
        }

        [NoCache]
        public ActionResult SignOut()
        {
            return View("SignOut");
        }

        public ActionResult ResetPassword()
        {
            return View("ResetPassword");
        }

        #endregion

    }
}
