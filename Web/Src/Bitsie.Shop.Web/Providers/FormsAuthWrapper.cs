using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Bitsie.Shop.Web.Providers 
{
    public class FormsAuthenticationWrapper: IAuth
    {
        public HttpCookie GetAuthCookie(string email)
        {
            return FormsAuthentication.GetAuthCookie(email, false);
        }

        public void DoAuth(string username, bool remember)
        {
            try
            {
                FormsAuthentication.SetAuthCookie(username, remember);
            }
            catch (Exception)
            {
            }
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
