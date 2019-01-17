using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bitsie.Shop.Services
{
    public class QueueService : IQueueService
    {
        private readonly IQueueRepository _queueRepository;
        private readonly IOrderService _orderService;
        private readonly IBitcoinService _bitcoinService;
        private readonly ILogService _logService;
        private readonly INotificationService _notificationService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IWalletAddressRepository _walletAddressRepository;

        public QueueService(IQueueRepository queueRepository,
            IOrderService orderService,
            ILogService logService,
            IBitcoinService bitcoinService,
            ISubscriptionService subscriptionService,
            INotificationService notificationService,
            IWalletAddressRepository walletAddressRepository)
        {
            _queueRepository = queueRepository;
            _orderService = orderService;
            _bitcoinService = bitcoinService;
            _logService = logService;
            _notificationService = notificationService;
            _subscriptionService = subscriptionService;
            _walletAddressRepository = walletAddressRepository;
        }

        public Queue GetQueueByGuid(string guid)
        {
            return _queueRepository.FindAll().FirstOrDefault(q => q.Guid == guid);
        }

        public IList<Queue> GetFailed()
        {
            return _queueRepository.FindAll().Where(q => q.Status == QueueStatus.Failed).ToList();
        }

        public void Save(Queue queue)
        {
            _queueRepository.Save(queue);
        }

        public Queue Enqueue(string guid, QueueAction action, string input, QueueStatus status, string url)
        {
            var queue = new Queue
            {
                Guid = guid,
                Action = action,
                Input = input,
                Status = status,
                Url = url
            };

            var existing = GetQueueByGuid(guid);
            if (existing != null)
            {
                throw new QueueExistsException("Queue already exists with guid " + guid);
            }

            Save(queue);
            return queue;
        }

        public void Process(Queue queue) {

            switch (queue.Action)
            {
                case QueueAction.ChainIpn:
                    ProcessChain(queue);
                    break;
                case QueueAction.BitpayIpn:
                    ProcessBitpay(queue);
                    break;
                case QueueAction.CoinbaseIpn:
                    ProcessCoinbase(queue);
                    break;
                case QueueAction.GoCoinIpn:
                    ProcessGoCoin(queue);
                    break;
                case QueueAction.Cron:
                    ProcessCron(queue);
                    break;
                case QueueAction.Subscriptions:
                    ProcessSubscriptions(queue);
                    break;
            }

            queue.Status = QueueStatus.Complete;
            _queueRepository.Save(queue);
        }

        private void ProcessSubscriptions(Queue queue)
        {
            var queueDate = queue.QueueDate;

            // Send out reminders for subscriptions about to expire in 7 days.
            _subscriptionService.SendSubscriptionReminders(queue.QueueDate.AddDays(7));

            // Send out reminders for subscriptions about to expire in 1 days.
            _subscriptionService.SendSubscriptionReminders(queue.QueueDate.AddDays(1));

            // Send out reminders for subscriptions that expire today
            _subscriptionService.SendSubscriptionReminders(queue.QueueDate);

            // Update subscriptions that have expired
            _subscriptionService.UpdateSubscriptionStatuses();

            // Unsuspend accounts
            if (queueDate.Day == 1) _subscriptionService.UnsuspendAccounts();
        }

        private void ProcessCron(Queue queue) {
            IList<string> processed = new List<string>();

            try
            {
                var orders = _orderService.GetOpenOrders();
                int blockHeight = _bitcoinService.GetBlockHeight();

                foreach (var o in orders)
                {
                    try
                    {
                        // Sync blockchain
                        if (!processed.Contains(o.PaymentAddress))
                        {
                            var transactions = _bitcoinService.GetTransactions(o.PaymentAddress);
                            _orderService.UpdateTransactions(o, transactions);
                            processed.Add(o.PaymentAddress);
                        }
                        _orderService.SyncOrder(o, blockHeight);
                    }
                    catch (Exception ex)
                    {
                        _logService.CreateLog(new Log
                        {
                            Category = LogCategory.Application,
                            Message = "Payout error: " + ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : ""),
                            Level = Domain.LogLevel.Error,
                            LogDate = DateTime.UtcNow
                        });

                        _notificationService.Notify("support@bitsie.com", "Bitsie Shop Error Has Occurred", "Error", new
                        {
                            Message = "Payout error: " + ex.Message,
                            StackTrace = ex.StackTrace,
                            InnerException = ex.InnerException == null ? "" : ex.InnerException.Message
                        });
                    }
                }

                var payout = _orderService.ProcessPayouts(orders);
                if (payout != null)
                {
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Message = "Payout sent for " + payout.PayoutAmount + ", tx: " + payout.TransactionId,
                        Level = Domain.LogLevel.Info,
                        LogDate = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Payout error: " + ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : ""),
                    Level = Domain.LogLevel.Error,
                    LogDate = DateTime.UtcNow
                });

                _notificationService.Notify("support@bitsie.com", "Bitsie Shop Error Has Occurred", "Error", new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? "" : ex.InnerException.Message
                });

                throw ex;
            }
        }

        private void ProcessCoinbase(Queue queue)
        {
            var qs = JObject.Parse(queue.Input);

            if (qs["order"] == null || qs["order"]["id"] == null) 
                throw new Exception("Coinbase order or order ID not supplied.");

            string orderNumber = qs["order"]["id"].ToString();
            var order = _orderService.GetOrderByNumber(orderNumber);
            if (order == null)  throw new Exception("Could not find Coinbase order number " + orderNumber);

            switch (qs["order"]["status"].ToString())
            {
                case "new":
                    order.Status = OrderStatus.Pending;
                    break;
                case "paid":
                    if (order.Status != OrderStatus.Paid)
                        _orderService.SendPaymentNotification(order.User.Email, order.User, order);
                    order.Status = OrderStatus.Paid;
                    break;
                case "confirmed":
                    order.Status = OrderStatus.Confirmed;
                    break;
                case "completed":
                    order.Status = OrderStatus.Complete;
                    break;
                case "expired":
                    order.Status = OrderStatus.Expired;
                    break;
                case "mispaid":
                    order.Status = OrderStatus.Partial;
                    break;
            }
            order.BtcPaid = long.Parse(qs["order"]["total_btc"].ToString()) / 100000000;
            _orderService.Save(order);
            _orderService.UpdateInternalRecords(order);
        }

        private void ProcessBitpay(Queue queue)
        {
            var qs = JObject.Parse(queue.Input);

            if (qs["id"] == null) throw new Exception("Bitpay order ID not supplied.");

            string orderNumber = qs["id"].ToString();
            var order = _orderService.GetOrderByNumber(orderNumber);
            if (order == null)
            {
                throw new Exception("Could not find Bitpay order number " + orderNumber);
            }

            switch (qs["status"].ToString())
            {
                case "new":
                    order.Status = OrderStatus.Pending;
                    break;
                case "paid":
                    if (order.Status != OrderStatus.Paid)
                        _orderService.SendPaymentNotification(order.User.Email, order.User, order);
                    order.Status = OrderStatus.Paid;
                    break;
                case "confirmed":
                    order.Status = OrderStatus.Confirmed;
                    break;
                case "complete":
                    order.Status = OrderStatus.Complete;
                    break;
                case "paidPartial":
                    order.Status = OrderStatus.Partial;
                    _orderService.SendPartialPaymentNotification(order);
                    break;
                case "expired":
                    order.Status = OrderStatus.Expired;
                    break;
                case "invalid":
                    order.Status = OrderStatus.Partial;
                    break;
            }
            order.BtcPaid = Decimal.Parse(qs["btcPaid"].ToString());
            _orderService.Save(order);
            _orderService.UpdateInternalRecords(order);
        }

        private void ProcessGoCoin(Queue queue)
        {
            var qs = JObject.Parse(queue.Input);

            if (qs["payload"] == null || qs["payload"]["id"] == null)
                throw new Exception("GoCoin order not supplied.");

            string orderNumber = qs["payload"]["id"].ToString();
            var order = _orderService.GetOrderByNumber(orderNumber);
            if (order == null)
                throw new Exception("Could not find GoCoin order number " + orderNumber);

            switch (qs["payload"]["status"].ToString())
            {
                case "unpaid":
                    order.Status = OrderStatus.Pending;
                    break;
                case "paid":
                    if (order.Status != OrderStatus.Paid)
                        _orderService.SendPaymentNotification(order.User.Email, order.User, order);
                    order.Status = OrderStatus.Paid;
                    break;
                case "invoice_confirmed":
                    order.Status = OrderStatus.Confirmed;
                    break;
                case "invoice_completed":
                case "ready_to_ship":
                    order.Status = OrderStatus.Complete;
                    break;
                case "merchant_review":
                case "paidPartial":
                    order.Status = OrderStatus.Partial;
                    _orderService.SendPartialPaymentNotification(order);
                    break;
                default:
                    order.Status = OrderStatus.Pending;
                    break;
            }

            //TODO: take json expires_at date and determine expiration
            DateTime expirationDate = order.OrderDate.AddMinutes(15);
            //Note: an invoice never passes into an 'Expired' state - the expiration window is based on the value of "expires_at" in the Invoice.
            if (order.Status == OrderStatus.Pending && DateTime.UtcNow > expirationDate)
            {
                order.Status = OrderStatus.Expired;
            }

            decimal btcBalanceDue = Decimal.Parse(qs["payload"]["crypto_balance_due"].ToString());

            order.BtcPaid = order.BtcTotal - btcBalanceDue;
            _orderService.Save(order);
            _orderService.UpdateInternalRecords(order);
        }

        private void ProcessChain(Queue queue)
        {
            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Processing chain queue: " + queue.Guid,
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info
            });

            var qs = JObject.Parse(queue.Input);

            string txId = qs["payload"]["transaction_hash"].ToString();
            string address = qs["payload"]["address"].ToString();

            var offline = _orderService.GetOfflineAddress(address);

            if (offline != null)
            {
                // This was a non-offline payment
                _orderService.AddOfflineOrder(address, txId);
            }
            else
            {
                // Check if HD wallet
                var walletAddress = _walletAddressRepository.FindAll().FirstOrDefault(a => a.Address == address);
                if (walletAddress != null)
                {
                    walletAddress.IsUsed = true;
                    _walletAddressRepository.Save(walletAddress);
                }

                // Online order
                var order = _orderService.GetOrderByAddress(address);
                var tran = _bitcoinService.GetTransaction(address, txId);
                _orderService.SyncTransaction(address, tran, order);
                _orderService.SyncOrder(order, _bitcoinService.GetBlockHeight());
                
            }
        }
    }
}
