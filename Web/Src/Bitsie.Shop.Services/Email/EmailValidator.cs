using System;
using System.Net.Mail;

namespace Bitsie.Shop.Services
{
    public static class EmailValidator
    {
        /// <summary>
        /// Indicates if the specified email address is valid.
        /// </summary>
        /// <param name="email">Address to validate</param>
        /// <returns></returns>
        public static bool IsValid(string email)
        {
            try
            {
                var address = new MailAddress(email);
            } catch(Exception)
            {
                return false;
            }
            return true;
        }
    }
}
