using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Models;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    public class ErrorController : Controller
    {

        public ActionResult Exception()
        {
            return View("Exception");
        }

        public ActionResult NotFound()
        {
            return View("NotFound");
        }
    }
}
