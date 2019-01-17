using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    public class IntegrationsController : BaseManageController
    {
        private readonly IUserService _userService;

        public IntegrationsController(IUserService userService)
        {
            _userService = userService;
        }

        public ActionResult Index(string type)
        {
            return View(type);
        }
    }
}
