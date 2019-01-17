using System;
using System.Configuration;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using Bitsie.Shop.Infrastructure.Mapping;
using Bitsie.Shop.Services;
using NHibernate.Tool.hbm2ddl;
using SharpArch.Domain.PersistenceSupport;
using SharpArch.NHibernate;
using Microsoft.Practices.ServiceLocation;
using Configuration = NHibernate.Cfg.Configuration;

namespace Bitsie.Shop.Build
{
    class Program
    {
        #region Fields

        private static Configuration _configuration;
        private static IUserService _userService;
        private static IPermissionRepository _permissionRepository;
        private static ILogRepository _logRepository;
        private static IWindsorContainer _container;

        #endregion

        #region Main Routine

        static void Main()
        {
            InitNHibernate();
            InitServiceLocator();

            Console.WriteLine("Connections initialized. What would you like to do?");
            Console.WriteLine("[D]emo Environment Setup");
            Console.WriteLine("[U]ser Interface Testing Setup");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Press a key...");
            bool validKey = false;

            while (!validKey)
            {
                string key = Console.ReadKey().Key.ToString().ToLower();
                switch(key)
                {
                    case "d":
                        validKey = true;
                        SetupDemoSite();
                        break;
                    case "u":
                        validKey = true;
                        SetupInterfaceTestSite();
                        break;
                    case "q":
                        validKey = true;
                        break;
                    default:
                        Console.Write("Invalid choice. Press a key...");
                        break;
                }
            }

            NHibernateSession.CloseAllSessions();
            NHibernateSession.Reset();
        }

        #endregion

        #region Setup Methods

        private static void SetupDemoSite()
        {
            CreateDb();
            CreatePermissions();
            CreateUsers(2);
        }

        private static void SetupInterfaceTestSite()
        {
            CreateDb();
            CreatePermissions();
            CreateUsers(300);
            CreateLogs(100);
        }

        private static void CreateLogs(int numLogs)
        {
            for (int i = 1; i <= numLogs; i++)
            {
                User user = _userService.GetUserById(1);
                var level = LogLevel.Debug;
                if (i <= 20) level = LogLevel.Debug;
                else if (i <= 40) level = LogLevel.Info;
                else if (i <= 80) level = LogLevel.Warning;
                else if (i <= 100) level = LogLevel.Error;

                var category = LogCategory.Application;
                if (i <= 30) category = LogCategory.Application;
                if (i <= 60) category = LogCategory.Security;
                else if (i <= 90) category = LogCategory.System;

                _logRepository.Save(new Log
                    {
                        Category = category,
                        IpAddress = "127.0.0." + i,
                        Level = level,
                        Details = "Log details " + i,
                        LogDate = DateTime.Now.AddDays(-1 * i),
                        Message = "Sample log " + i,
                        User = (i % 2 == 0) ? null : user
                    });
            }
        }

        private static void CreatePermissions()
        {
            _permissionRepository.Save(new Permission
            {
                Name = Permission.EditUsers,
                Description = "Edit users"
            });

            _permissionRepository.Save(new Permission
            {
                Name = Permission.EditOrders,
                Description = "Edit orders"
            });

            _permissionRepository.Save(new Permission
            {
                Name = Permission.EditProducts,
                Description = "Edit products"
            });
        }

        private static void CreateUsers(int numMembers)
        {
            for (int i = 1; i <= numMembers; i++)
            {
                var member = new User
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "member" + i + "@shop.bitsie.com",
                    Role = Role.Merchant,
                    ResetPasswordToken = "member" + i,
                    Status = UserStatus.Active,
                    Settings = new Settings
                    {
                        DailyMaximum = 1000m,
                        PaymentAddress = "1LwFEAjiU9Z1TqTLfmnrNs66zYPBAt4T6C", // Prod Wallet
                        EnableGratuity = true,
                        PaymentMethod = PaymentMethod.Bitcoin
                    },
                    Company = new Company
                    {
                        City = "Baltimore",
                        Industry = "Information Technology",
                        Name = "Honeypot Accounting",
                        Phone = "555-555-5555",
                        Street = "140 Branchwood Ct.",
                        State = "MD",
                        Zip = "21009",
                        Website = "http://www.honeypotaccounting.com"
                    }
                };
                _userService.GenerateUserPassword(member, "test");
                _userService.CreateUser(member);
                if (member.Id == 1)
                {
                    member.MerchantId = "bitsie";
                    _userService.UpdateUser(member);
                }
            }

            var admin = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "admin@shop.bitsie.com",
                Role = Role.Administrator,
                Status = UserStatus.Active,
                Company = new Company
                {
                    City = "Abingdon",
                    Industry = "Information Technology",
                    Name = "Bitsie",
                    Phone = "555-555-5555",
                    Street = "140 Branchwood Ct.",
                    State = "MD",
                    Zip = "21009",
                    Website = "http://www.bitsie.com"
                }
            };
            _userService.GenerateUserPassword(admin, "test");
            _userService.CreateUser(admin);
        }

        #endregion

        #region Helper Methods

        private static void InitNHibernate()
        {
            _configuration = NHibernateSession.Init(
                new SimpleSessionStorage(),
                new[] { "Bitsie.Shop.Infrastructure.dll" },
                new AutoPersistenceModelGenerator().Generate(),
                "../../../../Src/Bitsie.Shop.Web/NHibernate.config");
        }

        private static void InitServiceLocator()
        {
            // Setup Windsor for SharpArchitecture
            _container = new WindsorContainer();
            ComponentRegistrar.Initialize(_container);
            ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(_container));
            InitServices();
        }

        private static void InitServices()
        {
            _userService = _container.Resolve<IUserService>();
            _permissionRepository = _container.Resolve<IPermissionRepository>();
            _logRepository = _container.Resolve<ILogRepository>();
        }

        private static void CreateDb()
        {
            new SchemaExport(_configuration).Execute(false, true, false);

            RunScript("1.10-customer-dash.sql");
            RunScript("payout-balances.sql");
            RunScript("default-settings.sql");
            RunScript("bip39-wallet.sql");
        }

        private static void RunScript(string filename)
        {
            // Get path to item in Build/Scripts folder. 
            // Relative to executable path of compiled executable.
            var a = Assembly.GetEntryAssembly();
            var baseDir = System.IO.Path.GetDirectoryName(a.Location);
            var fullPath = System.IO.Path.Combine("../../../../Build/Scripts/", filename);
            var script = System.IO.File.ReadAllText(fullPath);
            var stringSeparators = new string[] { "\nGO" };
            var lines = script.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var query in lines)
            {
                var command = NHibernateSession.Current.Connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
