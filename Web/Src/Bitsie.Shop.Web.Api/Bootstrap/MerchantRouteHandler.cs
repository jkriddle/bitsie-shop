using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bitsie.Shop.Web.Api
{
    /// <summary>
    /// Custom handler that checks if the current URL is a merchant's custom URL,
    /// or if it is a Bitsie Shop page. If it is a merchant the request is routed to
    /// the merchant controller
    /// </summary>
    public class MerchantRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var url = requestContext.HttpContext.Request.Path.TrimStart('/');
            string[] path = url.Split('/');

            // Redirect legacy accounts
            if (path.Length > 0 && path[0].ToLower() == "checkout")
            {
                requestContext.HttpContext.Response.Clear();
                requestContext.HttpContext.Response.Redirect("/" + path[1] + "/checkout");
                requestContext.HttpContext.Response.End();
            }

            var userService = ServiceLocator.Current.GetInstance<IUserService>();
            var merchant = userService.GetUserByMerchantId(path[0]);
            string controller = String.IsNullOrEmpty(path[0]) ? "Home" : path[0];
            string action = path.Length > 1 && !String.IsNullOrEmpty(path[1]) ? path[1] : "Index";
            string id = path.Length > 2 ? path[2] : null;

            if (merchant != null)
            {
                // Merchant URL is like this: /NqZqYG/Customer/Signup/[id]
                // So we have to shift the path values over to the right
                controller = String.IsNullOrEmpty(path[1]) ? "Home" : path[1];
                action = path.Length > 2 && !String.IsNullOrEmpty(path[2]) ? path[2] : "Index";
                id = path.Length > 3 ? path[3] : null;
                FillRequest(action,
                    controller,
                    merchant.MerchantId,
                    "Merchant",
                    new string[] { "Bitsie.Shop.Web.Areas.Merchant.Controllers" },
                    requestContext);
            }
            else
            {
                FillRequest(action, controller, id, null, null, requestContext);
            }

            return base.GetHttpHandler(requestContext);
        }

        private static void FillRequest(string action, string controller, string id, string area, string[] namespaces, RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }


            if (namespaces != null && namespaces.Length > 0) requestContext.RouteData.DataTokens["namespaces"] = namespaces;
            requestContext.RouteData.DataTokens["area"] = area;
            requestContext.RouteData.Values["controller"] = controller;
            requestContext.RouteData.Values["action"] = action;
            requestContext.RouteData.Values["id"] = id;

            // disabling the namespace lookup fallback mechanism keeps this areas from accidentally picking up
            // controllers belonging to other areas
            bool useNamespaceFallback = (namespaces == null || namespaces.Length == 0);
            requestContext.RouteData.DataTokens["UseNamespaceFallback"] = useNamespaceFallback;
        }
    }
}