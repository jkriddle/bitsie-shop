using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bitsie.Shop.Web.Controllers
{
    public class CronController : BaseController
    {
        private IOrderService _orderService;
        private IBitcoinService _bitcoinService;
        private ILogService _logService;
        private INotificationService _notificationService;
        private readonly IQueueService _queueService;

        public CronController(IUserService userService, IOrderService orderService, IBitcoinService bitcoinService, 
            INotificationService notificationService, ILogService logService, 
            IQueueService queueService) : base(userService) {
            _orderService = orderService;
            _bitcoinService = bitcoinService;
            _logService = logService;
            _notificationService = notificationService;
            _queueService = queueService;
        }

        /// <summary>
        /// Run this every 5 minutes
        /// </summary>
        /// <param name="pass"></param>
        public void Dequeue(string pass)
        {
            if (pass != "tothemoon2015") throw new HttpException(401, "Not authorized.");

            // Create a guid of the current minute. This will prevent multiple requests from
            // processing queues multiple times.
            string guid = "Dequeue_" + DateTime.UtcNow.ToString("MMddyyyyHHmm");
            var queue = _queueService.Enqueue(guid, QueueAction.Cron, null, QueueStatus.Complete, Request.RawUrl);

            var queueList = _queueService.GetFailed();
            foreach (var q in queueList)
            {
                try
                {
                    _queueService.Process(q);
                    q.Status = QueueStatus.Complete;
                    _queueService.Save(q);
                }
                catch (Exception ex)
                {
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Message = "Could not process dequeue queue " + q.Id,
                        LogDate = DateTime.UtcNow,
                        Level = LogLevel.Info,
                        Details = ex.Message + ex.StackTrace
                    });

                    // Cancel after an hour of failure
                    if (q.QueueDate.AddHours(1) < DateTime.UtcNow) q.Status = QueueStatus.Cancelled;
                    else q.Status = QueueStatus.Failed;
                    _queueService.Save(q);
                }
            }

            _queueService.Save(queue);
        }

        /// <summary>
        /// Run this once per day
        /// </summary>
        /// <param name="pass"></param>
        public void UpdateSubscriptions(string pass)
        {
            string guid = "Subscriptions_" + DateTime.UtcNow.ToString("MMddyyyyHHmm");
            Enqueue(pass, QueueAction.Subscriptions, "subscriptions", guid);
        }

        /// <summary>
        /// Run this every 10 minutes
        /// </summary>
        /// <param name="pass"></param>
        public void Payout(string pass)
        {
            string guid = "Payout_" + DateTime.UtcNow.ToString("MMddyyyyHHmm");
            Enqueue(pass, QueueAction.Cron, "payout", guid);
        }

        private void Enqueue(string pass, QueueAction action, string label, string id)
        {
            if (pass != "tothemoon2015") throw new HttpException(401, "Not authorized.");

            var queue = _queueService.Enqueue(id, action, null, QueueStatus.Pending, Request.RawUrl);

            try
            {
                _queueService.Process(queue);
            }
            catch (Exception ex)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Could not process " + label + " queue " + queue.Id,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ex.Message + ex.StackTrace
                });
                queue.Status = QueueStatus.Failed;
                _queueService.Save(queue);
                throw new HttpException(500, "Could not process " + label + " queue " + queue.Id);
            }

            Response.Clear();
            Response.Write("*ok*");
            Response.End();
        }

    }
}
