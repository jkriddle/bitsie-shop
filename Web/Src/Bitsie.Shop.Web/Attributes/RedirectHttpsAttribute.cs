using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bitsie.Shop.Web.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class RedirectHttpsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!filterContext.HttpContext.Request.IsSecureConnection
                && !filterContext.HttpContext.Request.IsLocal
                && !string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"], "https",
                StringComparison.InvariantCultureIgnoreCase))
            {
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:"));
                filterContext.Result.ExecuteResult(filterContext);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}