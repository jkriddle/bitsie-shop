namespace Bitsie.Shop.Web.Bootstrap
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteRegistrar
    {
        public static void RegisterRoutesTo(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Product",                                              // Route name
                "{controller}/{action}/{id}/{item}",                           // URL with parameters
                new { controller = "Home", action = "Index", item = UrlParameter.Optional },  // Parameter defaults
                new[] { "Bitsie.Shop.Web.Controllers" }).RouteHandler = new MerchantRouteHandler();

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },  // Parameter defaults
                new[] { "Bitsie.Shop.Web.Controllers" }).RouteHandler = new MerchantRouteHandler();

        }
    }
}
