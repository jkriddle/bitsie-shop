using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Services
{
    /// <summary>
    /// Base class for notification services.
    /// </summary>
    public abstract class BaseNotificationService : INotificationService
    {

        #region Fields

        protected readonly ILogService LogService;

        #endregion

        #region Constructor

        protected BaseNotificationService(ILogService logService)
        {
            LogService = logService;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Send a notification that does not contain any placeholders
        /// </summary>
        /// <param name="toAddress">Email that will receive the notification</param>
        /// <param name="template">Template to be parsed</param>
        /// <returns></returns>
        public Task<bool> Notify(string toAddress, string subject, string template)
        {
            return Notify(toAddress, template, null);
        }

        /// <summary>
        /// Send a notification with placeholders replaced with specified parameters
        /// </summary>
        /// <param name="toAddress">Email that will receive the notification</param>
        /// <param name="template">Template to be parsed</param>
        /// <param name="messageParams">Object containing propreties to be replaced</param>
        /// <returns></returns>
        public abstract Task<bool> Notify(string toAddress, string subject, string template, object messageParams);

        #endregion
    }
}
