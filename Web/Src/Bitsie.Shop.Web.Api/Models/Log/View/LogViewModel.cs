using Bitsie.Shop.Domain;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Models;
using Newtonsoft.Json;

namespace Bitsie.Shop.Web.Api.Models
{
    public class LogViewModel : BaseViewModel
    {
        #region Fields

        private readonly Log _innerLog;

        #endregion

        #region Constructor

        public LogViewModel(Log log)
        {
            _innerLog = log;
        }

        #endregion

        #region Properties

        public int LogId { get { return _innerLog.Id; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Category { get { return _innerLog.Category.GetDescription(); } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Details { get { return _innerLog.Details; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Message { get { return _innerLog.Message; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string IpAddress { get { return _innerLog.IpAddress; } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string Level { get { return _innerLog.Level.GetDescription(); } }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string LogDate 
        {
            get { return _innerLog.LogDate.ToString("MM/dd/yyyy hh:mm:ss tt"); } 
        }
        public int? UserId
        {
            get
            {
                return _innerLog.User == null ? (int?)null : _innerLog.User.Id;
            }
        }
        [JsonConverter(typeof(SanitizeXssConverter))]
        public string UserEmail
        {
            get
            {
                return _innerLog.User == null ? string.Empty : _innerLog.User.Email;
            }
        }

        #endregion


    }
}