using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using Bitsie.Shop.Web.Attributes;
using Microsoft.Practices.ServiceLocation;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class ReportController : BaseManageController
    {
        public ActionResult Gratuity()
        {
            return View("Gratuity");
        }
    }
}
