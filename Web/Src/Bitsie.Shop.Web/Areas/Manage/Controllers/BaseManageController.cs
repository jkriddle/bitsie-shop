using System.Web.Mvc;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Attributes;
using Microsoft.Practices.ServiceLocation;
using Bitsie.Shop.Services;
using System.Web;
using System;

namespace Bitsie.Shop.Web.Areas.Manage.Controllers
{
    [RequiresAuthentication, RedirectHttpsAttribute]
    public class BaseManageController : Controller
    {
        #region Public Properties
        
        public User CurrentUser { get; set; }

        #endregion

        #region Overrides

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            // Check for web-based login
            var userService = ServiceLocator.Current.GetInstance<IUserService>();
            if (User.Identity.IsAuthenticated)
            {
                User user = userService.GetUserById(Int32.Parse(User.Identity.Name));
                CurrentUser = user;

                if (CurrentUser != null && CurrentUser.Status != UserStatus.Active && CurrentUser.Status != UserStatus.Suspended)
                {
                    throw new HttpException(401, "Account is not active.");
                }
            }
        }

        #endregion
    }
}