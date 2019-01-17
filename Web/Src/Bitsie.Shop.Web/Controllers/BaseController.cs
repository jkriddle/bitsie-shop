using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using System.IO;

namespace Bitsie.Shop.Web.Controllers
{
    [RedirectHttps]
    public class BaseController : Controller
    {
        #region Fields

        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Public Properties

        public User CurrentUser { get; set; }

        #endregion

        #region Overrides

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            if (requestContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string username = requestContext.HttpContext.User.Identity.Name;
                CurrentUser = _userService.GetUserById(Int32.Parse(username));
            }
        }

        public string RenderToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion

    }
}