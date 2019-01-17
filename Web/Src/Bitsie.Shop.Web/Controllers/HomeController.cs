using Bitsie.Shop.Api;
using Bitsie.Shop.Services;
using System;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            bool showManageLinks = true;
            if (User.Identity.IsAuthenticated)
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);
                if (user != null && user.Status != Domain.UserStatus.Active) showManageLinks = false;
            }
            ViewBag.ShowManageLinks = showManageLinks;
            return View();
        }

    }
}
