namespace Bitsie.Shop.Web.Api
{
    using System.Web.Mvc;
    using System.Web.Http;

    public class RouteRegistrar
    {
        public static void RegisterRoutesTo(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
