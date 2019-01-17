using System.Web.Mvc;
using System.Web.Security;

namespace Bitsie.Shop.Web.Attributes
{
    /// <summary>
    /// Verifies that user is logged into system.
    /// </summary>
    public class RequiresAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //redirect if not authenticated
            var controller = filterContext.Controller as Controller;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //use the current url for the redirect
                string redirectOnSuccess = filterContext.HttpContext.Request.Url.AbsolutePath;

                //send them off to the login page
                string redirectUrl = string.Format("?ReturnUrl={0}", redirectOnSuccess);
                string loginUrl = FormsAuthentication.LoginUrl + redirectUrl;
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}