using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Providers;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class ProductController : BaseManageController
    {
        private readonly IUserService _userService;

        public ProductController(IUserService userService)
        {
            _userService = userService;
        }

        [RequiresAuthentication]
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }

        public ActionResult Details()
        {
            return View("Details");
        }
    }
}
