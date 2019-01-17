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
using System.Net;
using System.Web.Http;
using System.Net.Http;

namespace Bitsie.Shop.Web.Api.Attributes
{
    /// <summary>
    /// Verifies that user is logged into system using the specified
    /// authorizatino token from the request headers
    /// </summary>
    public class RequiresApiAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // This attribute requires a user to be logged in via web or mobile device.
            // The attribute depends on the controller inheriting BaseApiController, which sets the "CurrentUser"
            // property as well as applys the user's settings

            // Check for web-based login
            var baseController = (BaseApiController)actionContext.ControllerContext.Controller;
            if (baseController.CurrentUser == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Could not authorize device." });
            }

        }
    }
}