using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Controllers;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class OfflineController : BaseManageController
    {
        private readonly IUserService _userService;

        public OfflineController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Print()
        {
            return View("Print");
        }
    }
}
