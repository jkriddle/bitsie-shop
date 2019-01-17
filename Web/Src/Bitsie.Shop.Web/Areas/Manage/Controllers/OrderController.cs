using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Attributes;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class OrderController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Details()
        {
            return View("Details");
        }
    }
}
