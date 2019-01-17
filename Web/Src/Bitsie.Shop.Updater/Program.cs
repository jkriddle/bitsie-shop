using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure.Mapping;
using Bitsie.Shop.Services;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using SharpArch.Domain.Events;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Updater
{
    class Program
    {

        private static IBitcoinService _bitcoinService;
        private static IWalletService _walletService;
        private static ISystemService _systemService;
        private static ILogService _logService;
        private static IOrderService _orderService;
        private static INotificationService _notificationService;
        private static IConfigService _configService;
        private static IQueueService _queueService;

        public static void Init()
        {
            InitializeServiceLocator();
            NHibernateInitializer.Instance().InitializeNHibernateOnce(InitialiseNHibernateSessions);
            _walletService = ServiceLocator.Current.GetInstance<IWalletService>();
            _systemService = ServiceLocator.Current.GetInstance<ISystemService>();
            _logService = ServiceLocator.Current.GetInstance<ILogService>();
            _orderService = ServiceLocator.Current.GetInstance<IOrderService>();
            _bitcoinService = ServiceLocator.Current.GetInstance<IBitcoinService>();
            _notificationService = ServiceLocator.Current.GetInstance<INotificationService>();
            _configService = ServiceLocator.Current.GetInstance<IConfigService>();
            _queueService = ServiceLocator.Current.GetInstance<IQueueService>();
        }

        public static void InitialiseNHibernateSessions()
        {
            NHibernateSession.ConfigurationCache = null; // new NHibernateConfigurationFileCache();
            string configPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "NHibernate.config");

            NHibernateSession.Init(new ThreadSessionStorage(),
                new[] { "Bitsie.Shop.Infrastructure.dll" },
                new AutoPersistenceModelGenerator().Generate(),
                configPath).BuildSessionFactory();
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        public static void InitializeServiceLocator()
        {
            var container = new WindsorContainer();

            // Initialize MVC application
            ComponentRegistrar.Initialize(container);

            // Setup service locator
            var windsorServiceLocator = new WindsorServiceLocator(container);
            DomainEvents.ServiceLocator = windsorServiceLocator;
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }


        static void Main(string[] args)
        {
            Init();

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Running Bitsie.Shop.Updater.exe...",
                Level = Domain.LogLevel.Info,
                LogDate = DateTime.UtcNow
            });

            try
            {
                Console.WriteLine("Sending payouts...");
                Payout();
                Console.WriteLine("Dequeue...");
                Dequeue();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Bitsie.Shop.Updater.exe error: " + ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException.Message : ""),
                    Level = Domain.LogLevel.Error,
                    LogDate = DateTime.UtcNow
                });

                _notificationService.Notify("support@bitsie.com", "Bitsie Shop Updater.exe Error Has Occurred", "Error", new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException == null ? "" : ex.InnerException.Message
                });
            }

            _logService.CreateLog(new Log
            {
                Category = LogCategory.Application,
                Message = "Bitsie.Shop.Updater.exe complete.",
                Level = Domain.LogLevel.Info,
                LogDate = DateTime.UtcNow
            });
        }

        private static void Dequeue()
        {
            // Create a guid of the current minute. This will prevent multiple requests from
            // processing queues multiple times.
            string guid = "Dequeue_" + DateTime.UtcNow.ToString("MMddyyyyHHmm");
            var queue = _queueService.Enqueue(guid, QueueAction.Cron, null, QueueStatus.Complete, "Bitsie.Shop.Updater.exe");

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

        private static void Payout()
        {
            Queue queue = null;
            try
            {
                // Create a guid of the current minute. This will prevent multiple requests from
                // sending multiple payouts. We can only have one payout being processed per minute max.
                string guid = "Payout_" + DateTime.UtcNow.ToString("MMddyyyyHHmm");
                queue = _queueService.Enqueue(guid, QueueAction.Cron, null, QueueStatus.Pending, "Bitsie.Shop.Updater.exe");
                _queueService.Process(queue);
            }
            catch (Exception ex)
            {
                if (queue != null)
                {
                    Console.WriteLine("Could not process payout queue " + queue.Id);
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Message = "Could not process payout queue " + queue.Id,
                        LogDate = DateTime.UtcNow,
                        Level = LogLevel.Error,
                        Details = ex.Message + ex.StackTrace
                    });
                    queue.Status = QueueStatus.Failed;
                    _queueService.Save(queue);
                    throw new Exception("Could not process payout queue " + queue.Id);
                }
                else
                {
                    Console.WriteLine(ex.Message);
                    _logService.CreateLog(new Log
                    {
                        Category = LogCategory.Application,
                        Message = "Bitsie.Shop.Updater.exe error: " + ex.Message,
                        LogDate = DateTime.UtcNow,
                        Level = LogLevel.Error,
                        Details = ex.StackTrace
                    });
                }
                throw ex;
            }
        }

    }
}
