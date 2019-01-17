using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Web.Providers;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class UserController : BaseManageController
    {
        private readonly IAuth _auth;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IAuth auth)
        {
            _userService = userService;
            _auth = auth;
        }

        [RequiresRole(Role.Administrator)]
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

        public ActionResult Settlement()
        {
            return View("Settlement");
        }

        public ActionResult Integrations()
        {
            return View("Integrations");
        }

        public ActionResult Design()
        {
            return View("Design", CurrentUser);
        }

        public ActionResult Tipsie()
        {
            return View("Tipsie", "Index");
        }

        /// <summary>
        /// Emulate a user's session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequiresAuthentication, RequiresRole(Role.Administrator)]
        public ActionResult Emulate(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null)
            {
                return new HttpNotFoundResult("User not found: " + id);
            }
            _auth.DoAuth(user.Id.ToString(), false);

            if (user.Role == Role.Tipsie) return RedirectToAction("Dashboard", "Tipsie", new { @area="" });
            else return RedirectToAction("Index", "Dashboard", null);
        }
    }
}
