using Bitsie.Shop.Web.Providers;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Security;

namespace Bitsie.Shop.Web.Areas.Api.Controllers
{
    /// <summary>
    /// This controller simply routes the traffic sent to /api/* over to the Bitsie Shop API.
    /// It acts as a proxy for all traffic to/from the API.
    /// </summary>
    public class ProxyController : ApiController
    {
        private readonly IAuth _auth;

        public ProxyController(IAuth auth)
        {
            _auth = auth;
        }

        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpHead, HttpOptions, HttpPatch]
        public object Route()
        {
            string url = HttpContext.Current.Request.Url.PathAndQuery;
            url = Regex.Replace(url, "/api", "", RegexOptions.IgnoreCase);
            return Relay(url);
        }

        private object Relay(string url)
        {
            var relayRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["ApiEndpoint"] + url);
            string postData = "";
            HttpContext.Current.Request.InputStream.Position = 0;
            using (var requestStream = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                postData = requestStream.ReadToEnd();
            }

            HttpWebRequest webRequest = relayRequest;
            webRequest.Method = HttpContext.Current.Request.HttpMethod;
            webRequest.ContentType = "application/x-www-form-urlencoded"; // HttpContext.Current.Request.ContentType;
            webRequest.ContentLength = postData.Length;
            webRequest.UserAgent = HttpContext.Current.Request.UserAgent;

            // Verify anti-CSR
            if (!IsSignInRequest(url) && webRequest.Method == "POST" && !HttpContext.Current.Request.UserAgent.ToLower().Contains("android"))
            {
                string cookieValue = HttpContext.Current.Request.Cookies["__RequestVerificationToken"].Value;
                string headerValue = HttpContext.Current.Request.Headers["__RequestVerificationToken"];
                AntiForgery.Validate(cookieValue, headerValue);
            }

            // Legacy authentication
            if (HttpContext.Current.Request.Cookies["authToken"] != null)
            {
                webRequest.Headers.Add("AuthToken", HttpUtility.UrlDecode(HttpContext.Current.Request.Cookies["authToken"].Value));
            }else if (HttpContext.Current.Request.Headers["authToken"] != "null")
            {
                webRequest.Headers.Add("AuthToken", HttpUtility.UrlDecode(HttpContext.Current.Request.Headers["authToken"]));
            }

            if (!string.IsNullOrEmpty(postData))
            {
                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                    requestWriter.Close();
                }
            }

            string responseData = null;
            try
            {
                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    responseReader.Close();
                    webRequest.GetResponse().Close();
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpResponseException(
                        new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            Content = new StringContent("Something went wrong.")
                        });
                }
                else
                {
                    throw new HttpResponseException(
                        new HttpResponseMessage(((HttpWebResponse)ex.Response).StatusCode)
                        {
                            Content = new StringContent(ex.Message)
                        });
                }
            }

            // Hack in order to set up a session for user logins on this site. V2 of API will not require a state like this,
            // but for now it removes the need to revamp how authentication is checked throughout the Manage area.
            if (IsSignInRequest(url))
            {
                try
                {
                    var json = JObject.Parse(responseData);
                    if (json["User"] != null && json["User"]["UserId"] != null)
                    {
                        _auth.DoAuth(json["User"]["UserId"].ToString(), true);
                    }
                }
                catch (Exception)
                {
                    // Fails because user was not authenticated.
                }
            }

            if (url.ToLower().Contains("user/signout"))
            {
                _auth.SignOut();

                // TODO - API is not stateless.
                // The below implementation prevents the API from being
                // stateless. A better implementation would be OAuth or some other
                // kerberos/token method, however for the time being...

                // clear authentication cookie
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                authCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(authCookie);

                // clear session cookie
                var sessionCookie = new HttpCookie("ASP.NET_SessionId", "");
                sessionCookie.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(sessionCookie);

                // clear token
                var authToken = new HttpCookie("authToken", "");
                authToken.Expires = DateTime.Now.AddYears(-1);
                HttpContext.Current.Response.Cookies.Add(authToken);
            }

            return JsonConvert.DeserializeObject(responseData);
        }

        private bool IsSignInRequest(string url)
        {
            return url.ToLower().Contains("user/signin") || url.ToLower().Contains("user/signup") || url.ToLower().Contains("user/forgotpassword");
        }

    }
}
