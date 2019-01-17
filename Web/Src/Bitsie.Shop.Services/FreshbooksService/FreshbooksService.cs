using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using System;

namespace Bitsie.Shop.Services
{
    public class FreshbooksService : IFreshbooksService
    {
        private User _freshbooksUser;
        private readonly IConfigService _configService;

        private FreshbooksApi _api;
        private FreshbooksApi Api
        {
            get
            {
                if (_api == null)
                {
                    _api = new FreshbooksApi(_freshbooksUser.Settings.FreshbooksApiUrl,
                        _configService.AppSettings("FreshbooksAccountName"),
                        _configService.AppSettings("FreshbooksOAuthToken"));
                    //_api.UseLegacyToken(_freshbooksUser.Settings.FreshbooksAuthToken);
                }
                return _api;
            }
        }

        public FreshbooksService(IConfigService configService)
        {
            _configService = configService;
        }

        public void SetAccount(User user)
        {
            _freshbooksUser = user;
            _api = null;
            if (!String.IsNullOrEmpty(user.Settings.FreshbooksAuthToken))
            {
                // Restore OAuth token state
                Api.SetTokenState(user.Settings.FreshbooksAuthToken);
            }
        }

        #region Authorization

        public Uri GetAuthorizationUrl(string redirectUrl)
        {
            Api.SetRedirectUrl(redirectUrl);
            return Api.GetAuthroizationUrl();
        }

        public void AuthorizeToken(Uri url)
        {
            Api.AuthorizeToken(url);
        }

        public void AuthorizeToken(string token)
        {
            Api.AuthorizeToken(token);
        }

        public string GetTokenState()
        {
            return Api.GetTokenState();
        }

        #endregion

        #region Invoicing

        public FreshbooksInvoice GetInvoiceById(ulong id)
        {
            return Api.GetInvoiceById(id);
        }

        public FreshbooksInvoice GetInvoiceByNumber(string number)
        {
            return Api.GetInvoiceByNumber(number);
        }

        #endregion

    }
}
