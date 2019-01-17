using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Api.Providers
{
    /// <summary>
    /// Wrapper for handling session data.
    /// </summary>
    public class SessionProvider : ISessionProvider
    {
        private readonly ControllerContext _context;

        public SessionProvider(ControllerContext context)
        {
            _context = context;
        }

        private HttpSessionStateBase CurrentSession
        {
            get
            {
                if (_context == null || _context.HttpContext == null || _context.HttpContext.Session == null)
                {
                    throw new Exception("User session does not exist or could not be retrieved.");
                }
                return _context.HttpContext.Session;
            }
        }

        /// <summary>
        /// Update a value in the session or create it if it does not exist.
        /// </summary>
        /// <param name="key">Item's key</param>
        /// <param name="value">Value of session item</param>
        private void SetSession(string key, object value)
        {
            if (CurrentSession[key] == null) CurrentSession.Add(key, value);
            else CurrentSession[key] = value;
        }

        /// <summary>
        /// Destroys a value in the current session.
        /// </summary>
        /// <param name="key">Key of item to destroy</param>
        private void RemoveSession(string key)
        {
            CurrentSession.Remove(key);
        }

        /// <summary>
        /// Emulate session of another user.
        /// </summary>
        /// <param name="userId">User ID to emulate</param>
        public void EmulateUser(int userId)
        {
            OriginalUserId = userId;
            IsEmulatedSession = true;
        }

        /// <summary>
        /// Returns session to state prior to emulating a user.
        /// </summary>
        public void RestoreOriginalUserSession()
        {
            RemoveSession("OriginalUserId");
            IsEmulatedSession = false;
        }
        

        /// <summary>
        /// If current user is an administrator logged in on another person's behalf, this is the administrator
        /// username they were logged in as when they initiated the emulation of the other user. This way
        /// admins can return back to their dashboard when they are finished.
        /// </summary>
        public int OriginalUserId
        {
            get
            {
                var val = CurrentSession["OriginalUserId"];
                if (val == null) return 0;
                return (int)val;
            }
            set { SetSession("OriginalUserId", value); }
        }


        /// <summary>
        /// Returns true if the current user is an administrator logged in on another person's behalf.
        /// </summary>
        public bool IsEmulatedSession
        {
            get {
                var val = CurrentSession["IsEmulatedSession"];
                if (val == null) return false;
                return (bool)val;
            }
            set { SetSession("IsEmulatedSession", value); }
        }

    }
}