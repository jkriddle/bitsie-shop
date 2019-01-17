using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Security;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Controllers;
using Microsoft.Practices.ServiceLocation;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Bitsie.Shop.Web.Api.Attributes
{
    /// <summary>
    /// Verifies that user has the specified role
    /// </summary>
    public class ApiRequiresRole : ActionFilterAttribute
    {
        private const string _authTokenParamKey = "AuthToken";

        public Role[] AllowedRoles { get; set; }

        public ApiRequiresRole(Role role)
        {
            AllowedRoles = new Role[] { role };
        }

        public ApiRequiresRole(params Role[] roles)
        {
            AllowedRoles = roles;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            bool valid = false;
            string authTokenParam = HttpContext.Current.Request.Headers[_authTokenParamKey];

            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // Check web session
                foreach (Role role in AllowedRoles)
                {
                    if (System.Web.HttpContext.Current.User.IsInRole(role.ToString())) valid = true;
                }
            } else if (authTokenParam != null)
            {
                // Check device session
                var userService = ServiceLocator.Current.GetInstance<IUserService>();

                User user = userService.Authenticate(authTokenParam);
                foreach (Role role in AllowedRoles)
                {
                    if (role == user.Role) valid = true;
                }
            }
            
            if (!valid) throw new HttpException(401, "Unable to validate your device credentials.");
        }
    }
}