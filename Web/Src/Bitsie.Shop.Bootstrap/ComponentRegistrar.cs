using System;
using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Bitsie.Shop.Services;
using SharpArch.Domain.Commands;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Contracts.Repositories;
using Bitsie.Shop.Api;


namespace Bitsie.Shop.Infrastructure
{
    public class ComponentRegistrar
    {
        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddGenericRepositoriesTo(container);
            AddCustomRepositoriesTo(container);
            AddServicesTo(container);
            AddMembershipTo(container);
        }

        private static void AddMembershipTo(IWindsorContainer container)
        {
            container.Register(
                    Component.For<IAuth>().ImplementedBy<FormsAuthenticationWrapper>()
                );
        }

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
                Component.For<ITemplateService>()
                    .ImplementedBy<TemplateService>());

            container.Register(
                Component.For<IOrderService>()
                    .ImplementedBy<OrderService>());

            container.Register(
                    Component.For(typeof(IWalletApi))
                        .ImplementedBy(typeof(BlockchainWalletApi))
                        .Named("walletApi").DependsOn(new
                        {
                            guid = ConfigurationManager.AppSettings["BlockchainGuid"],
                            password = ConfigurationManager.AppSettings["BlockchainPassword"],
                            secondPassword = ConfigurationManager.AppSettings["BlockchainSecondPassword"]
                        }));

            container.Register(
               Component.For<INotificationService>()
                .ImplementedBy<EmailNotificationService>()
                .DependsOn(new
                {
                    fromEmail = ConfigurationManager.AppSettings["EmailFromAddress"],
                    fromName = ConfigurationManager.AppSettings["EmailFromName"]
                }));

            container.Register(
                AllTypes
                    .FromAssemblyNamed("Bitsie.Shop.Services")
                    .Pick().If(t => t.Name.EndsWith("Service"))
                    .WithService
                    .FirstNonGenericCoreInterface("Bitsie.Shop.Services"));
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                AllTypes
                    .FromAssemblyNamed("Bitsie.Shop.Infrastructure")
                    .BasedOn(typeof(ILinqRepositoryWithTypedId<,>))
                    .WithService.FirstNonGenericCoreInterface("Bitsie.Shop.Infrastructure"));
        }

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

            container.Register(
                    Component.For(typeof(IBitcoinApi))
                        .ImplementedBy(typeof(BlockchainInfoApi))
                        .Named("bitcoinApi"));
        }

    }
}