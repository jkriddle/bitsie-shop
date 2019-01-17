using System.Web;
using System.Web.Http;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Attributes;
using Bitsie.Shop.Domain;
using System;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitsie.Shop.Web.Controllers
{
    [NoCache]
    public class IPNController : BaseController
    {
        #region Fields

        private readonly IConfigService _configService;
        private readonly IOrderService _orderService;
        private readonly ILogService _logService;
        private readonly IBitcoinService _bitcoinService;
        private readonly IQueueService _queueService;

        #endregion

        #region Constructor

        public IPNController(IUserService userService,
            ILogService logService,
            IConfigService configService,
            IBitcoinService bitcoinService,
            IQueueService queueService,
            IOrderService orderService) : base(userService)
        {
            _configService = configService;
            _orderService = orderService;
            _bitcoinService = bitcoinService;
            _logService = logService;
            _queueService = queueService;
        }

        #endregion

        /// <summary>
        /// Coinbase IPN
        /// </summary>
        public void Coinbase()
        {
            Request.InputStream.Position = 0;
            var input = new StreamReader(Request.InputStream).ReadToEnd();

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Received Coinbase IPN transaction request.",
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = input
            });

            var qs = JObject.Parse(input);
            if (qs["order"] == null || qs["order"]["id"] == null) return;

            string orderNumber = qs["order"]["id"].ToString();
            CheckQueue(orderNumber, QueueAction.CoinbaseIpn, input, QueueStatus.Pending, Request.RawUrl);
        }

        /// <summary>
        /// Bitpay IPN
        /// </summary>
        public void Bitpay()
        {
            Request.InputStream.Position = 0;
            var input = new StreamReader(Request.InputStream).ReadToEnd();
            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Received Bitpay IPN transaction request.",
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = input
            });

            var qs = JObject.Parse(input);
            if (qs["id"] == null) return;

            string orderNumber = qs["id"].ToString();
            CheckQueue(orderNumber, QueueAction.BitpayIpn, input, QueueStatus.Pending, Request.RawUrl);
        }

        /// <summary>
        /// GoCoin IPN
        /// </summary>
        public void GoCoin()
        {

            Request.InputStream.Position = 0;
            var input = new StreamReader(Request.InputStream).ReadToEnd();

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Received GoCoin IPN transaction request.",
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = input
            });

            var qs = JObject.Parse(input);
            if (qs["payload"] == null || qs["payload"]["id"] == null) return;
            if (qs["event"].ToString() != "invoice_payment_received") return;

            string orderNumber = qs["payload"]["id"].ToString();
            CheckQueue(orderNumber, QueueAction.GoCoinIpn, input, QueueStatus.Pending, Request.RawUrl);
        }
       
        /// <summary>
        /// Chain.com update
        /// </summary>
        public void Update()
        {
            Request.InputStream.Position = 0;
            var input = new StreamReader(Request.InputStream).ReadToEnd();
            if (String.IsNullOrEmpty(input))
            {
                return;
            }

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Received Chain.com IPN transaction request.",
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = input
            });

            var qs = JObject.Parse(input);
            if (qs["payload"] == null || qs["payload"]["transaction_hash"] == null)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "No payload or transaction hash found.",
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = input
                });
                return;
            }

            string txId = qs["payload"]["transaction_hash"].ToString();
            CheckQueue(txId, QueueAction.ChainIpn, input, QueueStatus.Pending, Request.RawUrl);
        }

        private void CheckQueue(string guid, QueueAction action, string input, QueueStatus status, string url)
        {
            Queue queue = null;

            try
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Adding to queue: " + guid,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = input
                });

                queue = _queueService.Enqueue(guid, action, input, status, url);
            }
            catch (QueueExistsException)
            {
                // Queue already exists
                Response.Clear();
                Response.Write("*ok*");
                Response.End();
                return;
            }

            try {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Processing queue: " + guid,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = input
                });
                _queueService.Process(queue);
            } catch(Exception ex) {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Failed to process IPN queue " + queue.Id,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ex.Message + ex.StackTrace
                });
                queue.Status = QueueStatus.Failed;
                _queueService.Save(queue);
                throw new HttpException(500, "Failed to process IPN queue " + queue.Id);
            }

            Response.Clear();
            Response.Write("*ok*");
            Response.End();
        }

    }

}
