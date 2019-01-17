using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    public class PageController : Controller
    {
        public new ActionResult View(string id)
        {
            return base.View(id);
        }
    }
}
