using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class DashboardController : BaseManageController
    {
        public ActionResult Index()
        {
            if (CurrentUser != null && CurrentUser.Role == Domain.Role.Administrator)
            {
                return View("AdminIndex");
            }
            return View("Index");
        }
    }
}
