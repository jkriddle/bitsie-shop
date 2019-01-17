using System.Web.Http;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Api";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
                "Api_default",
                "Api/{*path}",
                new { controller = "Proxy", action = "Route", id = UrlParameter.Optional }
            );
        }
    }
}
