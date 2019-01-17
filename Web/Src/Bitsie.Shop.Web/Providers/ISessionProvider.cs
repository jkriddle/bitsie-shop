using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Providers
{
    public interface ISessionProvider
    {
        void EmulateUser(int userId);
        void RestoreOriginalUserSession();
        int OriginalUserId { get; set; }
        bool IsEmulatedSession { get; set; }
    }
}