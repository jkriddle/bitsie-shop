using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Helpers
{
    public static class ConfigHelper
    {

        /// <summary>
        /// Retrieves a value from the web.config file's app settings.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

    }
}