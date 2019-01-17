using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Bitsie.Shop.Web.Api.Providers 
{
    public interface IAuth
    {
        HttpCookie GetAuthCookie(string email);
        void DoAuth(string username, bool remember);
        void SignOut();
    }
}