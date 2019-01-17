using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Helpers
{
    public static class QuerystringHelper
    {
        public static string Get(string name)
        {
            if (HttpContext.Current.Request.QueryString[name] == null) return String.Empty;
            return HttpContext.Current.Request.QueryString[name].ToString();
        } 
    }
}