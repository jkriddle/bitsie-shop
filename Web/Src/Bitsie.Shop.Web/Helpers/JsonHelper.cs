using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Helpers
{
    /// <summary>
    /// Helper to assist with JSON conversion and serialization to views.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Convert a .NET object to a JSON object. Used to output data into the view that will be used
        /// by other JavaScript functionality or views.
        /// </summary>
        /// <param name="ob">Object to convert</param>
        /// <returns>Serialized JSON object as a string</returns>
        public static HtmlString ConvertObject(object ob)
        {
            return new HtmlString(JsonConvert.SerializeObject(ob));
        } 

    }
}