using Bitsie.Shop.Api;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public class OrderService : IOrderService
    {
        private readonly IConfigService _configService;
        private readonly IOrderRepository _orderRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IBitcoinService _bitcoinService;
        private readonly IWalletApi _walletApi;
        private readonly INotificationService _notificationService;
        private readonly IOfflineAddressRepository _offlineAddressRepository;
        private readonly IMessageApi _messageApi;
        private readonly ISystemService _systemService;
        private readonly ILogService _logService;
        private readonly IInvoiceService _invoiceService;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IHdWalletService _hdWalletService;

        public OrderService(IConfigService configService, 
            IOrderRepository orderRepository,
            IWalletApi walletApi, IPayoutRepository payoutRepository,
            ITransactionRepository transactionRepository,
            IBitcoinService bitcoinService,
            IInvoiceService invoiceService,
            IOfflineAddressRepository offlineAddressRepository,
            IMessageApi messageApi,
            INotificationService notificationService,
            IInvoiceRepository invoiceRepository,
            ILogService logService,
            ISystemService systemService,
            IUserRepository userRepository,
            ISubscriptionRepository subscriptionRepository,
            IHdWalletService hdWalletService)
        {
            _orderRepository = orderRepository;
            _configService = configService;
            _walletApi = walletApi;
            _payoutRepository = payoutRepository;
            _notificationService = notificationService;
            _transactionRepository = transactionRepository;
            _offlineAddressRepository = offlineAddressRepository;
            _logService = logService;
            _bitcoinService = bitcoinService;
            _messageApi = messageApi;
            _systemService = systemService;
            _invoiceService = invoiceService;
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
            _subscriptionRepository = subscriptionRepository;
            _hdWalletService = hdWalletService;
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrder(Order order, string callbackUrl)
        {
            var api = GetApi(order);
            PaymentMethod? paymentMethod = order.User.Settings.PaymentMethod;

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Creating order for number " + order.OrderNumber + " user " + order.User.Id + " amount of " + order.Total,
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                User = order.User,
                Details = "Payment method type is " + (paymentMethod.HasValue ? paymentMethod.Value.ToString() : " none.")
            });

            switch (paymentMethod)
            {
                case PaymentMethod.Trezor:
                    _hdWalletService.CreateOrder(order);
                    break;
                case PaymentMethod.Wallet32:
                    _hdWalletService.CreateOrder(order);
                    break;
                case PaymentMethod.Bitpay:
                case PaymentMethod.Coinbase:
                case PaymentMethod.GoCoin:
                    api.CreateOrder(order, callbackUrl);
                    break;
                case PaymentMethod.Bitcoin:
                    _walletApi.CreateOrder(order);

                    // Don't add another hook if an offline address (we already have hooks created during user registration
                    if (order.OrderType != OrderType.OfflineAddress) _bitcoinService.CreateWebhook(order.PaymentAddress, 0);
                    break;
            }
            _orderRepository.Save(order);

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Level = LogLevel.Info,
                Message = "Order " + order.OrderNumber + " saved successfully for address " + order.PaymentAddress,
                LogDate = DateTime.UtcNow,
                User = order.User,
                Details = ""
            });
        }

        private IPaymentProcessorApi GetApi(Order order)
        {
            IPaymentProcessorApi api = null;
            User payoutUser = order.User;
            PaymentMethod? paymentMethod;

            // If the order is for a Tipsie account, and that Tipsie account has a parent merchant, we want
            // the order to be processed by the merchant's payout provider
            if (order.User.Role == Role.Tipsie && order.User.Merchant != null)
            {
                payoutUser = order.User.Merchant;
                paymentMethod = order.User.Merchant.Settings.PaymentMethod.Value;
            }
            else
            {
                paymentMethod = order.User.Settings.PaymentMethod;
            }

            switch (paymentMethod)
            {
                case PaymentMethod.Bitpay:
                    api = new BitpayApi(payoutUser.Settings.BitpayApiKey);
                    break;
                case PaymentMethod.Coinbase:
                    api = new CoinbaseApi(payoutUser.Settings.CoinbaseApiKey, payoutUser.Settings.CoinbaseApiSecret);
                    break;
                case PaymentMethod.GoCoin:
                    var goCoinApi = new GoCoinApi(_configService.AppSettings("GoCoinApiKey"), _configService.AppSettings("GoCoinApiSecret"));
                    goCoinApi.SetAccessToken(payoutUser.Settings.GoCoinAccessToken);
                    api = goCoinApi;
                    break;
            }
            return api;
        }

        /// <summary>
        /// Update orders to mark the payout as being sent
        /// </summary>
        /// <param name="order">Order to update</param>
        public Payout ProcessPayouts(IList<Order> orders)
        {
            var payments = new List<BitcoinPayment>();
            Payout payout = null;
            IList<Order> processed = new List<Order>();
            foreach (var order in orders)
            {
                // If not confirmed, carry on...
                if (order.Status != OrderStatus.Confirmed) continue;

                // Types of payouts
                // 1. Merchant that receives bitcoin and holds bitcoin
                // 2. Tipsie that receives bitcoin
                // 3. Tipsie that receives bitcoin through merchant processor
                var offlineAddress = GetOfflineAddress(order.PaymentAddress);
                string address = order.User.Settings.PaymentAddress;
                PaymentMethod? paymentMethod = order.User.Settings.PaymentMethod;
                if (order.User.Role == Role.Tipsie && order.User.Merchant != null)
                {
                    // For tipsie accounts under a merchant account, use the merchant's
                    // payment method (i.e. merchant gets tips, employee gets paycheck)
                    paymentMethod = order.User.Merchant.Settings.PaymentMethod;
                    address = order.User.Merchant.Settings.PaymentAddress;
                }

                // If user uses a processor, and this was NOT an offline address, simply mark it as complete.
                // We don't want to have a customer pay bitpay and then us send more money to bitpay!
                if (paymentMethod != PaymentMethod.Bitcoin && offlineAddress == null)
                {
                    order.Status = OrderStatus.Complete;
                    _orderRepository.Save(order);
                    continue;
                }

                if (!paymentMethod.HasValue)
                {
                    string message = "User " + order.User.Id + " received payment to offline address " + order.PaymentAddress + " but do not have a payment method set on their account. Order number " + order.OrderNumber;
                    // Now pay the payment processor or order
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Level = LogLevel.Info,
                        Message = message,
                        LogDate = DateTime.UtcNow,
                        Details = ""
                    });
                    throw new Exception(message);
                }

                BitcoinPayment existing = payments.FirstOrDefault(p => p.Address == address);
                switch (paymentMethod)
                {
                    // If the order is for a merchant that holds bitcoin, send it directly
                    case PaymentMethod.Bitcoin:
                        if (existing != null)
                        {
                            existing.Amount += order.BtcPaid;
                        }
                        else
                        {
                            payments.Add(new BitcoinPayment
                            {
                                Address = address,
                                Amount = order.BtcPaid
                            });
                            processed.Add(order);
                        }
                        
                        break;
                    default:
                        var api = GetApi(order);
                        try
                        {
                            var merchantOrder = new Order
                            {
                                Description = order.Description,
                                Gratuity = order.Gratuity,
                                Subtotal = order.Subtotal,
                                Status = OrderStatus.Pending,
                                Total = order.Total
                            };

                            api.CreateOrder(merchantOrder, _configService.AppSettings("IPNCallbackUrl"));

                            // Now pay the payment processor or order
                            _logService.CreateLog(new Log
                            {
                                Category = LogCategory.Application,
                                Level = LogLevel.Info,
                                Message = "Forwarding payment for order " + order.OrderNumber + " to payment address + " + merchantOrder.PaymentAddress,
                                LogDate = DateTime.UtcNow,
                                Details = ""
                            });

                            existing = payments.FirstOrDefault(p => p.Address == order.PaymentAddress);
                            if (existing != null)
                            {
                                existing.Amount += merchantOrder.BtcTotal;
                            }
                            else
                            {
                                payments.Add(new BitcoinPayment
                                {
                                    Address = merchantOrder.PaymentAddress,
                                    Amount = merchantOrder.BtcTotal
                                });
                                processed.Add(order);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Now pay the payment processor or order
                            _logService.CreateLog(new Log
                            {
                                Category = LogCategory.Application,
                                Level = LogLevel.Info,
                                Message = "Unable to create merchant forwarding order for " + order.OrderNumber + ": " + ex.Message,
                                LogDate = DateTime.UtcNow,
                                Details = ""
                            });
                        }
                        break;
                }
            }

            if (payments.Count > 0)
            {
                string txId = _walletApi.SendMany(payments);
                payout = AddPayout(processed, 0, null, txId);
                try
                {
                    SendBitcoinPayoutNotifications(payout);
                    SendAdminPayoutNotification(payout);
                }
                catch (Exception ex)
                {
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Level = LogLevel.Info,
                        Message = "Unable to createsend payout notifications: " + ex.Message + "\r\n" + ex.StackTrace,
                        LogDate = DateTime.UtcNow,
                        Details = ""
                    });
                }
            }

            return payout;
        }

        private Payout AddPayout(IList<Order> orders, decimal fee, string confirmationNumber, string txId)
        {
            decimal orderTotal = orders.Sum(o => o.Total);
            decimal payoutFee = Math.Round(orderTotal * (fee / 100), 2, MidpointRounding.AwayFromZero);
            decimal payoutAmount = Math.Round(orderTotal - payoutFee, 2, MidpointRounding.AwayFromZero);

            var payout = new Payout
            {
                ConfirmationNumber = confirmationNumber,
                TransactionId = txId,
                DateProcessed = DateTime.UtcNow,
                PayoutAmount = payoutAmount,
                PayoutFee = payoutFee
            };

            _payoutRepository.Save(payout);

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Complete;
                order.Payout = payout;
                _orderRepository.Save(order);
            }

            return payout;
        }


        /// <summary>
        /// Save order data
        /// </summary>
        /// <param name="order"></param>
        public void Save(Order order)
        {
            _orderRepository.Save(order);
        }

        /// <summary>
        /// Retrieve a single order
        /// </summary>
        /// <param name="id">Order's unique ID</param>
        /// <returns>Matching order</returns>
        public Order GetOrderById(int id)
        {
            return _orderRepository.FindOne(id);
        }


        /// <summary>
        /// Retrieve a single order
        /// </summary>
        /// <param name="id">Order's unique ID</param>
        /// <returns>Matching order</returns>
        public Order GetOrderByAddress(string address)
        {
            return _orderRepository.FindAll().OrderByDescending(a => a.OrderDate).FirstOrDefault(a => a.PaymentAddress == address);
        }

        /// <summary>
        /// Retrieve an offline address
        /// </summary>
        /// <param name="address">Bitcoin address</param>
        /// <returns></returns>
        public OfflineAddress GetOfflineAddress(string address)
        {
            return _offlineAddressRepository.FindAll().FirstOrDefault(a => a.Address == address && a.Status == OfflineAddressStatus.Active);
        }

        /// <summary>
        /// Retrieve a single order
        /// </summary>
        /// <param name="id">Order's unique number</param>
        /// <returns>Matching order</returns>
        public Order GetOrderByNumber(string number)
        {
            return _orderRepository.FindAll().FirstOrDefault(o => o.OrderNumber == number);
        }

        public IList<Order> GetOpenOrders()
        {
            return _orderRepository.FindAll().Where(o => o.Status == OrderStatus.Pending
                || o.Status == OrderStatus.Partial
                || o.Status == OrderStatus.Confirmed
                || o.Status == OrderStatus.Paid).ToList();
        }

        /// <summary>
        /// Add a transaction to the local database
        /// </summary>
        /// <param name="address"></param>
        /// <param name="transaction"></param>
        /// <param name="order"></param>
        public void SyncTransaction(string address, BitcoinTransaction transaction, Order order = null)
        {
            var existingTran = _transactionRepository.FindAll()
                                        .FirstOrDefault(t => t.TxId == transaction.TxId
                                        && t.Address == address);
            if (existingTran == null) existingTran = new Transaction();
            existingTran.Address = address;
            existingTran.Amount = transaction.Value;
            existingTran.Block = transaction.Block;
            existingTran.Order = order;
            existingTran.Category = transaction.Category;
            existingTran.TransactionDate = transaction.TransactionDate;
            existingTran.TxId = transaction.TxId;
            _transactionRepository.Save(existingTran);

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Transaction synced: " + existingTran.TxId,
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = "Address: " + address + ", Amount: " + transaction.Value
            });
        }

        public decimal UpdateTransactions(Order order, IList<BitcoinTransaction> transactions)
        {
            decimal totalBtcPaid = 0;
            if (transactions.Count == 0) return 0;

            foreach (var tran in transactions)
            {
                var existingTran =_transactionRepository.FindAll().FirstOrDefault(t => t.TxId == tran.TxId 
                                        && t.Address == tran.Address);

                // We only want transactions that we have on record! Otherwise transactions from re-used addresses
                // will cause problems
                if (existingTran == null) continue;

                // Add transaction to local system
                SyncTransaction(tran.Address, tran, order);

                // Find transactions created after this order, but within the past two hours.
                // In case payment provider re-uses addresses, we don't want future payments
                // to be recorded to the balance paid.
                if (tran.TransactionDate >= order.OrderDate.AddMinutes(-15)
                    && tran.TransactionDate < order.OrderDate.AddHours(2))
                {
                    totalBtcPaid += tran.Value;
                }
            }
            return totalBtcPaid;
        }

        /// <summary>
        /// Sync all open orders and process them accordingly.
        /// </summary>
        public void SyncAll()
        {
            var orders = GetOpenOrders();
            foreach (var o in orders)
            {
                var transactions = _bitcoinService.GetTransactions(o.PaymentAddress);
                UpdateTransactions(o, transactions);
                SyncOrder(o, _bitcoinService.GetBlockHeight());
            }
        }

        public void UpdateInternalRecords(Order order)
        {
            // If freshbooks invoice, update accordingly.
            if (order.FreshbooksInvoiceId.HasValue)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Freshbooks payment received for Freshbooks invoice " + order.FreshbooksInvoiceId.Value,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = "Order number " + order.OrderNumber
                });
                if (order.Status == OrderStatus.Paid) AddFreshbooksPayment(order);
            }

            // If bitsie shop invoice, update accordingly
            if (order.Invoice != null)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Payment received for Bitsie invoice ID " + order.Invoice.Id,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = "Order number " + order.OrderNumber
                });

                if (order.Status == OrderStatus.Paid) order.Invoice.Status = InvoiceStatus.Paid;
                else if (order.Status == OrderStatus.Partial) order.Invoice.Status = InvoiceStatus.Partial;
                _invoiceRepository.Save(order.Invoice);
            }

            // If subscription, update/renew
            if (order.Subscription != null && order.Status == OrderStatus.Paid)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Subscription " + order.Subscription.Id + " has been renewed",
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = "Order number " + order.OrderNumber
                });

                // If not expired extend future expiration date. Otherwise extend from current date.
                DateTime expirationDate = order.Subscription.DateExpires;
                if (expirationDate <= DateTime.UtcNow) expirationDate = DateTime.UtcNow;
                switch (order.Subscription.Term)
                {
                    case SubscriptionTerm.Monthly:
                        expirationDate = expirationDate.AddMonths(1);
                        break;
                    case SubscriptionTerm.Yearly:
                        expirationDate = expirationDate.AddYears(1);
                        break;
                }

                order.Customer.Subscription = order.Subscription;
                order.Subscription.DateRenewed = DateTime.UtcNow;
                order.Subscription.DateExpires = expirationDate;
                order.Subscription.Status = SubscriptionStatus.Active;
                if (order.Customer.Status == UserStatus.Suspended)
                {
                    order.Customer.Status = UserStatus.Active;
                }
                _subscriptionRepository.Save(order.Subscription);
                _orderRepository.Save(order);
                _userRepository.Save(order.Customer);

                _notificationService.Notify(order.Customer.Email, "Bitsie Shop Subscription Upgrade Receipt", 
                    "SubscriptionUpgraded", new
                {
                    Plan = order.Subscription.Type.GetDescription(),
                    OrderDate = order.OrderDate,
                    Description = order.Description,
                    Total = order.Total,
                    BtcTotal = order.BtcTotal,
                    PaymentAddress = order.PaymentAddress,
                    StatusDescription = order.Status.GetDescription()
                });
            }

            // Check number of transactions to lock accounts
            if (order.User.Subscription == null || order.User.Subscription.Type != SubscriptionType.Unlimited)
            {
                int numOrders = GetNumOrders(order.User.Id, DateTime.UtcNow.Month, DateTime.UtcNow.Year);
                int maxOrders = Int32.Parse(_configService.AppSettings("Subscription.Starter.Transactions"));
                string nextMonth = DateTime.UtcNow.AddMonths(1).ToString("MMMM");
                string plan = "Starter";
                if (order.User.Subscription != null)
                {
                    maxOrders = Int32.Parse(_configService.AppSettings("Subscription." + order.User.Subscription.Type + ".Transactions"));
                    plan = order.User.Subscription.Type.ToString();
                }

                if (numOrders >= maxOrders)
                {
                    // Account suspended
                    _notificationService.Notify(order.User.Email, "Bitsie Shop Transaction Limit - 100% Used", "SubscriptionSuspended", new
                    {
                        NumOrders = numOrders,
                        MaxOrders = maxOrders,
                        Plan = plan,
                        NextMonth = nextMonth
                    });
                    order.User.Status = UserStatus.Suspended;
                    _userRepository.Save(order.User);
                }
                else if (numOrders == Math.Floor(maxOrders * .90))
                {
                    // Nearing maximum
                    _notificationService.Notify(order.User.Email, "Bitsie Shop Transaction Limit - 90% Used", "SubscriptionNearingLimit", new
                    {
                        NumOrders = numOrders,
                        MaxOrders = maxOrders,
                        Plan = plan,
                        NextMonth = nextMonth
                    });
                }
            }
        }

        /// <summary>
        /// Retrieve number of orders processed in a given month/year
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public int GetNumOrders(int userId, int month, int year)
        {
            return _orderRepository.FindAll().Count(o => 
                o.OrderDate.Month == month 
                && o.OrderDate.Year == year 
                && o.User.Id == userId
                && (o.Status == OrderStatus.Complete || o.Status == OrderStatus.Confirmed 
                        || o.Status == OrderStatus.Paid || o.Status == OrderStatus.Partial));
        }

        /// <summary>
        /// Checks the order's status against the blockchain address
        /// </summary>
        /// <param name="order"></param>
        /// <param name="blockHeight">Current blockchain block height</param>
        public void SyncOrder(Order order, int blockHeight)
        {
            var transactions = _transactionRepository.FindAll().Where(r => r.Address == order.PaymentAddress 
                && r.Category == TransactionCategory.Receive);

            // First sync the transactions
            // Must use ToList() or else the precision of the decimal fails!
            decimal totalBtcPaid = transactions.ToList().Sum(t => t.Amount );

            // If order is closed out, take no additional action 
            if (order.Status == OrderStatus.Confirmed
                || order.Status == OrderStatus.Expired
                || order.Status == OrderStatus.Refunded) return;

            // If order is marked as partial or pending
            bool dirty = false;
            if (order.Status == OrderStatus.Pending
                || order.Status == OrderStatus.Partial)
            {
                if (totalBtcPaid >= order.BtcTotal)
                {
                    // Paid in full (or more)
                    order.BtcPaid = totalBtcPaid;
                    order.Status = OrderStatus.Paid;
                    dirty = true;
                    SendPaymentNotification(order.User.Email, order.User, order);
                    UpdateInternalRecords(order);
                } else if (totalBtcPaid < order.BtcTotal && totalBtcPaid > 0) {
                    // Partial payment
                    order.BtcPaid = totalBtcPaid;
                    order.Status = OrderStatus.Partial;
                    dirty = true;
                    UpdateInternalRecords(order);
                }

                // Now check for an expired payment
                if ((order.Status == OrderStatus.Pending || order.Status == OrderStatus.Partial)
                            && DateTime.UtcNow > order.OrderDate.AddMinutes(15))
                {
                    if (order.Status == OrderStatus.Partial)
                    {
                        SendPartialPaymentNotification(order);
                    }

                    dirty = true;
                    order.Status = OrderStatus.Expired;
                }
            }

            // If order is marked as paid, update the confirmations
            if (order.Status == OrderStatus.Paid)
            {
                int? latestBlock = transactions.Max(t => t.Block);
                int requiredConfirmations = 1;
                if (latestBlock.HasValue
                    && (blockHeight - requiredConfirmations) >= latestBlock.Value)
                {
                    dirty = true;
                    order.Status = OrderStatus.Confirmed;
                }
            }

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Order synced: " + order.Id,
                LogDate = DateTime.UtcNow,
                Level = LogLevel.Info,
                Details = "Status: " + order.Status + ", BTC Paid: " + order.BtcPaid
            });

            if (dirty) _orderRepository.Save(order);

        }

        private void AddFreshbooksPayment(Order order)
        {
            string customerAccount = order.User.Settings.FreshbooksApiUrl;
            string developerAccount = _configService.AppSettings("FreshbooksAccountName");
            string authToken = _configService.AppSettings("FreshbooksOAuthToken");
            var api = new FreshbooksApi(customerAccount, developerAccount, authToken);
            api.SetTokenState(order.User.Settings.FreshbooksAuthToken);

            try
            {
                ulong paymentId = api.AddPayment(order);
                order.FreshbooksPaymentId = Convert.ToInt64(paymentId);
            }
            catch (Exception ex)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Unable to make Freshbooks payment for order " + order.OrderNumber,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ex.Message + "\r\n" + ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Retrieve a paged list of orders
        /// </summary>
        /// <param name="filter">Query to filter orders</param>
        /// <param name="currentPage">Current page number</param>
        /// <param name="numPerPage">Number of items per page</param>
        /// <returns></returns>
        public IPagedList<Order> GetOrders(OrderFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<Order> orders = _orderRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<Order>(orders, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Create an order for a transaction that was sent to an offline address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="txId"></param>
        /// <returns></returns>
        public Order AddOfflineOrder(string address, string txId)
        {
            // Update our local DB with the transactions from this address
            BitcoinTransaction transaction = _bitcoinService.GetTransaction(address, txId);

            if (transaction == null)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Transaction not found with TxId " + txId,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ""
                });
                throw new Exception("Transaction not found with TxId " + txId);
            }

            var offline = _offlineAddressRepository.FindAll().FirstOrDefault(a => a.Address == address);
            if (offline == null)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Offline address payment detect but address is not used: " + address,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ""
                });
                throw new Exception("Offline address not used: " + address);
            }

            // Check to ensure this order isn't just a change address
            var isChange = transaction.Inputs.Count(i => i.Address == address);
            if (isChange > 0)
            {
                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Payment made to address " + address + " was detected as a change address payment",
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = ""
                });
                return null;
            }

            // If we don't already have an order for this transaction, create a new one.
            decimal price = _bitcoinService.GetMarketPrice();
            var order = new Order
            {
                Subtotal = transaction.Value * price,
                Gratuity = 0,
                BtcPaid = transaction.Value,
                User = offline.User,
                OrderDate = transaction.TransactionDate,
                Total = transaction.Value * price,
                BtcTotal = transaction.Value,
                OrderType = OrderType.OfflineAddress,
                PaymentAddress = address,
                ExchangeRate = price
            };

            // If the order is for a Tipsie account, all Tipsie orders are gratuity. Mark accordingly.
            if (order.User.Role == Role.Tipsie)
            {
                order.Gratuity = order.Total;
                order.Subtotal = 0;
            }

            // check if we already have this order
            var existingTransaction = _transactionRepository.FindAll().FirstOrDefault(t => t.TxId == txId);
            if (existingTransaction != null && existingTransaction.Order != null) return existingTransaction.Order;

            // Do this now so future calls aren't processed.
            SyncTransaction(address, transaction);

            // Save this order to the system. If payment method is not set it means the
            // account is a Tipsie account, so we're just saving the raw transaction.
            CreateOrder(order, _configService.AppSettings("IPNCallbackUrl"));

            // Update the address accordingly (don't use the processor's address)
            order.PaymentAddress = address;
            order.Status = OrderStatus.Paid;
            _orderRepository.Save(order);

            SyncTransaction(address, transaction, order);

            foreach (var email in offline.EmailList)
            {
                // Send email message
                SendPaymentNotification(email, offline.User, order);
            }

            foreach (var phone in offline.TextList)
            {
                // Send txt message
                _messageApi.SendSms(phone, "You have received a Bitsie Shop payment of " + order.Total.ToString("C") + " to address " + address);
            }

            return order;
        }

        #region Notifications

        private void SendAdminPayoutNotification(Payout payout)
        {
            var orders = _orderRepository.FindAll().Where(p => p.Payout == payout);

            if (orders.Count() > 0)
            {
                StringBuilder table = new StringBuilder();

                _notificationService.Notify(_configService.AppSettings("NotificationEmail"), 
                    "Payout Report: #" + payout.Id, 
                    "AdminPayoutReport", new
                {
                    Payout = payout,
                    Orders = orders
                });
            }
        }

        private void SendBitcoinPayoutNotifications(Payout payout)
        {
            var orders = _orderRepository.FindAll().Where(p => p.Payout == payout);

            if (orders.Count() > 0)
            {
                foreach (var o in orders)
                {
                    _notificationService.Notify(o.User.Email, 
                        "Bitsie Shop Payment for Order " + o.OrderNumber, 
                        "PayoutReport", 
                        new
                        {
                            PayoutId = payout.Id.ToString(),
                            DateProcessed = payout.DateProcessed.ToShortDateString() + " " + payout.DateProcessed.ToShortTimeString(),
                            Total = o.Total.ToString("C"),
                            OrderNumber = o.OrderNumber,
                            BtcTotal = o.BtcTotal.ToString("N9"),
                            Fee = payout.PayoutFee.ToString(),
                            ConfirmationNumber = (String.IsNullOrEmpty(payout.ConfirmationNumber) ? "" : payout.ConfirmationNumber),
                            TxId = (String.IsNullOrEmpty(payout.TransactionId) ? "" : payout.TransactionId),
                            PaymentAddress = o.PaymentAddress
                        });
                }
            }
        }

        public void SendPaymentNotification(string recipient, User user, Order order)
        {
            string template = "PaymentReceived";
            if (user.Settings.PaymentMethod == PaymentMethod.Bitcoin)
            {
                template = "BtcPaymentReceived";
            }
            _notificationService.Notify(recipient, "Bitsie Shop Payment Received", template, new
                {
                    Email = order.User.Email,
                    Total = order.Total.ToString("C"),
                    BtcTotal = order.BtcTotal.ToString("N6"),
                    MerchantId = order.User.MerchantId,
                    OrderNumber = order.OrderNumber
                });
        }

        public void SendPartialPaymentNotification(Order order)
        {
            _notificationService.Notify(order.User.Email, 
                "Bitsie Shop Partial Payment Received", 
                "PartialPaymentReceived", new
                        {
                            Email = order.User.Email,
                            Total = order.Total.ToString("C"),
                            BtcPaid = order.BtcPaid.ToString("N6"),
                            BtcTotal = order.BtcTotal.ToString("N6"),
                            MerchantId = order.User.MerchantId,
                            OrderNumber = order.OrderNumber
                        });
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate an order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="validationDictionary"></param>
        /// <returns></returns>
        public bool ValidateOrder(Order order, IValidationDictionary validationDictionary)
        {
            var requestDictionary = new ValidationDictionary();
            if (order.Subtotal <= 0)
            {
                requestDictionary.AddError("Subtotal", "Order subtotal must be greater than 0.");
            }
            if (order.Total <= 0)
            {
                requestDictionary.AddError("Total", "Order total must be greater than 0.");
            }
            validationDictionary.Merge(requestDictionary);
            return requestDictionary.IsValid;
        }

        #endregion
    }
}
