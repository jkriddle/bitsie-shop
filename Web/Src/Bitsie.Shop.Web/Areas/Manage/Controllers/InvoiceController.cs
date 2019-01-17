using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Attributes;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    [RequiresRole(Role.Merchant, Role.Administrator)]
    public class InvoiceController : BaseManageController
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult Edit()
        {
            return View("Edit");
        }

        public ActionResult Create()
        {
            return View("Create");
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
