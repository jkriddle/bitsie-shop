using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Attributes;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    [RequiresRole(Role.Merchant, Role.Administrator)]
    public class TipsieController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Card()
        {
            return View("Card");
        }

        public ActionResult Create()
        {
            return View("Create");
        }

        public ActionResult Print()
        {
            return View("Print");
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }

        public ActionResult Details()
        {
            return View("Details");
        }

        public ActionResult Setup()
        {
            return View("Setup");
        }
    }
}
