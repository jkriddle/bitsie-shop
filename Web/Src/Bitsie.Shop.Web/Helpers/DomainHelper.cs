using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Helpers
{
    public static class DomainHelper
    {

        public static string AbsoluteUrl(string path, bool forceHttps = false, bool encodeUrl = false)
        {
            // Already an absolute URL
            if (path.IndexOf("://") > -1)
            {
                if (encodeUrl) return HttpUtility.UrlEncode(path);
                return path;
            }

            string serverUrl = "";

            if (!String.IsNullOrEmpty(path)) serverUrl = VirtualPathUtility.ToAbsolute(path);

            string newUrl = serverUrl;
            var domain = HttpContext.Current.Request.ServerVariables["HTTP_HOST"];
            Uri originalUri = HttpContext.Current.Request.Url;

            newUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + domain + newUrl;

            if (encodeUrl) newUrl = HttpUtility.UrlEncode(newUrl);
            return newUrl;
        } 

    }
}