using System.Net.Http;
using System.Web;
using System.Web.Http;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Microsoft.Practices.ServiceLocation;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Services.CSV;
using System.Collections.Generic;
using System;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [RedirectHttpsAttribute, ApiExceptionAttribute]
    public class BaseApiController : ApiController
    {
        #region Fields

        protected readonly IUserService UserService;
        protected readonly ILogService LogService;

        #endregion

        #region Constructor

        public BaseApiController(IUserService userService, ILogService logService)
        {
            UserService = userService;
            LogService = logService;
        }

        #endregion

        #region Public Properties

        public User CurrentUser { get; set; }

        #endregion

        #region Public Methods

        public string GetClientIp(HttpRequestMessage request)
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null) 
                return null;

            // @note This will not work in a "self hosted" environment or
            // if the API is separated from the MVC framework.
            // If self hosted, use the commented lines below instead.
            // @see http://stackoverflow.com/questions/9565889/get-the-ip-address-of-the-remote-host
            return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            #region Self-Hosted Method

            /*if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else
            {
                return null;
            }*/
            
            #endregion

        }

        /// <summary>
        /// Generate a CSV and send it to the browser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="items"></param>
        public void Export<T>(string filename, IList<T> items)
        {
            string attachment = "attachment; filename=" + filename + ".csv";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", attachment);
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            CsvHelper.Write<T>(HttpContext.Current.Response.OutputStream, items);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        #endregion

        #region Overrides

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            // Check for web-based login
            var orderService = ServiceLocator.Current.GetInstance<IOrderService>();
            var userService = ServiceLocator.Current.GetInstance<IUserService>();
            var baseController = (BaseApiController)controllerContext.Controller;
            if (baseController.User.Identity.IsAuthenticated)
            {
                User user = userService.GetUserById(Int32.Parse(baseController.User.Identity.Name));
                baseController.CurrentUser = user;
                return;
            }

            // Check for mobile login
            string authTokenParam = HttpContext.Current.Request.Headers["authToken"];
            if (authTokenParam != null)
            {
                // Verify user
                User user = userService.Authenticate(authTokenParam);
                if (user != null)
                {
                    baseController.CurrentUser = user;
                }
            }

            base.Initialize(controllerContext);
        }

        #endregion

    }
}