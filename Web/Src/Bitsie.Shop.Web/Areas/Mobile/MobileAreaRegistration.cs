using System.Web.Mvc;

namespace Bitsie.Shop.Web.Areas.Mobile
{
    public class MobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Mobile_default",
                "Mobile/{controller}/{action}/{id}",
                new { controller = "App", action = "Index", id = UrlParameter.Optional },  // Parameter defaults                
                new[] { "Bitsie.Shop.Web.Areas.Mobile.Controllers" } 
            );
        }
    }
}
