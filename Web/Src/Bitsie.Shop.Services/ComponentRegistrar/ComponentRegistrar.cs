using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Bitsie.Shop.Services;
using SharpArch.Domain.Commands;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using SharpArch.Web.Mvc.Castle;
using Bitsie.Shop.Api;
using Bitsie.Shop.Infrastructure;


namespace Bitsie.Shop.Services
{
    public class ComponentRegistrar
    {
        /// <summary>
        /// Initialize default container
        /// </summary>
        /// <param name="container"></param>
        public static void Initialize(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddServicesTo(container);
        }

        /// <summary>
        /// Add an implementation to the registrar
        /// </summary>
        /// <typeparam name="I">Interface</typeparam>
        /// <typeparam name="T">Implementation of interface to use</typeparam>
        /// <param name="container">Registrar container</param>
        public static void Add<I, T>(IWindsorContainer container)
        {
            container.Register(
                    Component.For(typeof(I)).ImplementedBy(typeof(T))
                );
        }

        /// <summary>
        /// Add default services to container
        /// </summary>
        /// <param name="container"></param>
        private static void AddServicesTo(IWindsorContainer container)
        {
            container.Register(
               Component.For<IConfigService>()
                .ImplementedBy<ConfigService>()
                .DependsOn(new
                {
                    appSettings = ConfigurationManager.AppSettings
                }));

            var minLogLevel =
                (Domain.LogLevel)Enum.Parse(typeof(Domain.LogLevel), ConfigurationManager.AppSettings["MinLogLevel"]);

            container.Register(
               Component.For<ILogService>()
                .ImplementedBy<LogService>()
                .DependsOn(new
                {
                    minLevel = minLogLevel
                }));

            container.Register(
                Component.For<IEmailService>()
                    .ImplementedBy<EmailService>());

            container.Register(
                    Component.For(typeof(IMessageApi))
                        .ImplementedBy(typeof(TwilioApi))
                        .Named("messageApi").DependsOn(new
                        {
                            fromNumber = ConfigurationManager.AppSettings["TwilioNumber"],
                            sid = ConfigurationManager.AppSettings["TwilioSid"],
                            authToken = ConfigurationManager.AppSettings["TwilioAuthToken"]
                        }));

            container.Register(
                Component.For<IOrderService>()
                    .ImplementedBy<OrderService>());

            container.Register(
                    Component.For(typeof(IWalletApi))
                        .ImplementedBy(typeof(BlockchainWalletApi))
                        .Named("walletApi").DependsOn(new
                        {
                            guid = ConfigurationManager.AppSettings["WalletGuid"],
                            password = ConfigurationManager.AppSettings["WalletPassword"],
                            secondPassword = String.Empty
                        }));

            container.Register(
               Component.For<INotificationService>()
                .ImplementedBy<EmailNotificationService>()
                .DependsOn(new
                {
                    fromEmail = ConfigurationManager.AppSettings["EmailFromAddress"],
                    fromName = ConfigurationManager.AppSettings["EmailFromName"],
                    templateDirectory = ConfigurationManager.AppSettings["NotificationTemplatePath"]
                }));

            container.Register(
                AllTypes
                    .FromAssemblyNamed("Bitsie.Shop.Services")
                    .Pick().If(t => t.Name.EndsWith("Service"))
                    .WithService
                    .FirstNonGenericCoreInterface("Bitsie.Shop.Services"));
        }

        /// <summary>
        /// Add application-specific repositories to container
        /// </summary>
        /// <param name="container"></param>
        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
            
            container.Register(
               Component.For<IReportRepository>()
                .ImplementedBy<ReportRepository>());

            container.Register(
                    Component.For(typeof(IBitcoinApi))
                        .ImplementedBy(typeof(ChainApi))
                        .Named("bitcoinApi").DependsOn(new
                        {
                            apiKey = ConfigurationManager.AppSettings["ChainApiKey"],
                            apiSecret = ConfigurationManager.AppSettings["ChainApiSecret"],
                            webhook = ConfigurationManager.AppSettings["ChainWebhookID"],
                        }));

            container.Register(
                AllTypes
                    .FromAssemblyNamed("Bitsie.Shop.Infrastructure")
                    .BasedOn(typeof(ILinqRepositoryWithTypedId<,>))
                    .WithService.FirstNonGenericCoreInterface("Bitsie.Shop.Infrastructure"));

        }

        /// <summary>
        /// Add standard repositories to container
        /// </summary>
        /// <param name="container"></param>
        private static void AddGenericRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                Component.For(typeof(IEntityDuplicateChecker))
                    .ImplementedBy(typeof(EntityDuplicateChecker))
                    .Named("entityDuplicateChecker"));

            container.Register(
                Component.For(typeof(INHibernateRepository<>))
                    .ImplementedBy(typeof(NHibernateRepository<>))
                    .Named("nhibernateRepositoryType")
                    .Forward(typeof(IRepository<>)));

            container.Register(
                Component.For(typeof(INHibernateRepositoryWithTypedId<,>))
                    .ImplementedBy(typeof(NHibernateRepositoryWithTypedId<,>))
                    .Named("nhibernateRepositoryWithTypedId")
                    .Forward(typeof(IRepositoryWithTypedId<,>)));

            container.Register(
                    Component.For(typeof(ISessionFactoryKeyProvider))
                        .ImplementedBy(typeof(DefaultSessionFactoryKeyProvider))
                        .Named("sessionFactoryKeyProvider"));

            container.Register(
                    Component.For(typeof(ICommandProcessor))
                        .ImplementedBy(typeof(CommandProcessor))
                        .Named("commandProcessor"));
        }

    }
}