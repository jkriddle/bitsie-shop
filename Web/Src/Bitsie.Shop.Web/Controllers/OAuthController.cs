using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    [RequiresAuthentication]
    public class OAuthController : BaseController
    {
        private readonly IFreshbooksService _freshbooksService;
        private readonly IUserService _userService;
        private readonly IConfigService _configService;

        public OAuthController(IFreshbooksService freshbooksService, IConfigService configService,
            IUserService userService) : base(userService)
        {
            _freshbooksService = freshbooksService;
            _userService = userService;
            _configService = configService;
        }
        
        public ActionResult GoCoin(string code = "", bool disable = false)
        {
            if (CurrentUser == null) return Redirect("/");

            if (!disable && String.IsNullOrEmpty(code))
            {
                var gc = new GoCoinApi(_configService.AppSettings("GoCoinApiKey"), _configService.AppSettings("GoCoinApiSecret"));
                var redirect = gc.Authenticate(_configService.AppSettings("GoCoinRedirectUrl"));
                return Redirect(redirect);
            }

            if (disable)
            {
                CurrentUser.Settings.GoCoinAccessToken = null;
                CurrentUser.Settings.PaymentMethod = null;
                _userService.UpdateUser(CurrentUser);
            }
            else if (!String.IsNullOrEmpty(code))
            {
                var gc = new GoCoinApi(_configService.AppSettings("GoCoinApiKey"), _configService.AppSettings("GoCoinApiSecret"));
                var token = gc.GetAccessToken(code, _configService.AppSettings("GoCoinRedirectUrl"));
                CurrentUser.Settings.GoCoinAccessToken = token;
                CurrentUser.Settings.PaymentMethod = PaymentMethod.GoCoin;
                _userService.UpdateUser(CurrentUser);
            }
            return RedirectToAction("Index", "Integrations", new { @area = "Manage", @type="gocoin" });
        }

        public ActionResult Freshbooks()
        {
            if (!String.IsNullOrEmpty(Request.Url.Query))
            {
                // Authorize the token based on the query-string values in this request
                _freshbooksService.AuthorizeToken(Request.Url);

                // Save the auth token for later
                CurrentUser.Settings.FreshbooksAuthToken = _freshbooksService.GetTokenState();
                _userService.UpdateUser(CurrentUser);

                // Send back to the user profile page
                return RedirectToAction("Index", "Integrations", new { @area = "Manage", @type = "freshbooks", @connected = true });
            }
            return View();
        }

        [HttpPost]
        public void Freshbooks(string Url)
        {
            CurrentUser.Settings.FreshbooksApiUrl = Url;
            _freshbooksService.SetAccount(CurrentUser);
            _userService.UpdateUser(CurrentUser);

            // Not authenticated
            // Redirect the user to their freshbooks.com OAuth logon page, and have it redirect
            // back here once complete
            Uri redirect = _freshbooksService.GetAuthorizationUrl(Request.Url.AbsoluteUri);
            Response.Redirect(redirect.AbsoluteUri);
        }

    }
}
