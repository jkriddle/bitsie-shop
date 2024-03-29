using System.Web;
using System.Web.Http;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Bootstrap;
using Bitsie.Shop.Web.Controllers;
using Bitsie.Shop.Infrastructure.Mapping;
using SharpArch.Domain.Events;
using Microsoft.Practices.ServiceLocation;
using SharpArch.NHibernate;
using SharpArch.NHibernate.Web.Mvc;
using SharpArch.Web.Mvc.Castle;
using SharpArch.Web.Mvc.ModelBinder;
using Bitsie.Shop.Infrastructure;
using Bitsie.Shop.Web.Providers;
using Bitsie.Shop.Web.Attributes;
using System.Web.Helpers;

namespace Bitsie.Shop.Web
{
    
    /// <summary>
    /// Represents the MVC Application
    /// </summary>
    /// <remarks>
    /// For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </remarks>
    public class MvcApplication : HttpApplication
    {
        #region Fields

        private WebSessionStorage _webSessionStorage;

        #endregion

        #region HttpApplication Events/Overrides

        /// <summary>
        /// Due to issues on IIS7, the NHibernate initialization must occur in Init().
        /// But Init() may be invoked more than once; accordingly, we introduce a thread-safe
        /// mechanism to ensure it's only initialized once.
        /// See http://msdn.microsoft.com/en-us/magazine/cc188793.aspx for explanation details.
        /// </summary>
        public override void Init()
        {
            base.Init();
            this._webSessionStorage = new WebSessionStorage(this);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            NHibernateInitializer.Instance().InitializeNHibernateOnce(this.InitialiseNHibernateSessions);
            AddXFrameHeader();
        }

        private void AddXFrameHeader()
        {
            // Anything placed in here will ALLOW x-frame usage
            string[] whitelist = {
                                       "^((?!manage).)*/product(.*)", // product pages except in manage area
                                       "^((?!manage).)*/checkout(.*)", // checkout pages except in manage area
                                   };

            bool allowed = false;

            foreach (var reg in whitelist)
            {
                if (new System.Text.RegularExpressions.Regex(reg).IsMatch(HttpContext.Current.Request.Url.AbsolutePath)) allowed = true; 
            }
            
            if (allowed) HttpContext.Current.Response.AddHeader("X-Frame-Options", "ALLOWALL");
            else HttpContext.Current.Response.AddHeader("X-Frame-Options", "DENY");
        }

        protected void Application_Start()
        {

#if DEBUG
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
#endif

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            ModelBinders.Binders.DefaultBinder = new SharpModelBinder();
            ModelValidatorProviders.Providers.Add(new ClientDataTypeModelValidatorProvider());

            InitializeServiceLocator();

            AreaRegistration.RegisterAllAreas();
            RouteRegistrar.RegisterRoutesTo(RouteTable.Routes);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            AutoMapperServiceBootstrap.Init();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            try
            {
                // Attempt to log the error
                var logService = ServiceLocator.Current.GetInstance<ILogService>();
                logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = exception.Message + " at " + HttpContext.Current.Request.Url.ToString(),
                    Details = exception.StackTrace,
                    Level = LogLevel.Error,
                });

            }
            catch (Exception)
            {

            }

#if DEBUG
            //throw exception;
#else
            try
            {
                var notificationService = ServiceLocator.Current.GetInstance<INotificationService>();
                var configService = ServiceLocator.Current.GetInstance<IConfigService>();
                notificationService.Notify("support@bitsie.com", "Bitsie Shop Error Has Occurred", "Error", new
                        {
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            InnerException = exception.InnerException == null ? "" : exception.InnerException.Message
                        });

                var app = (MvcApplication)sender;
                var context = app.Context;
                var ex = app.Server.GetLastError();
                context.Response.Clear();
                context.ClearError();
                var httpException = ex as HttpException;

                var routeData = new RouteData();
                routeData.Values["controller"] = "error";
                routeData.Values["exception"] = ex;
                routeData.Values["action"] = "exception";
                if (httpException != null)
                {
                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            routeData.Values["action"] = "NotFound";
                            break;
                    }
                }
                IController controller = new ErrorController();
                controller.Execute(new RequestContext(new HttpContextWrapper(context), routeData));

            }
            catch (Exception)
            {

            }
#endif
        }

        #endregion

        #region Private Helper Methods

        private void InitialiseNHibernateSessions()
        {
            NHibernateSession.ConfigurationCache = null; // new NHibernateConfigurationFileCache();

             var config = NHibernateSession.Init(
                this._webSessionStorage,
                new[] { Server.MapPath("~/bin/Bitsie.Shop.Infrastructure.dll") },
                new AutoPersistenceModelGenerator().Generate(),
                Server.MapPath("~/NHibernate.config"));
            SessionFactory.Instance = config.BuildSessionFactory();

            GlobalConfiguration.Configuration.Filters.Add(new NhSessionManagementAttribute());
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from
        /// WindsorController to the container.  Also associate the Controller
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator()
        {
            var container = new WindsorContainer();

            // Initialize MVC application
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
            container.RegisterControllers(typeof(HomeController).Assembly);
            ComponentRegistrar.Initialize(container);
            ComponentRegistrar.Add<IAuth, FormsAuthenticationWrapper>(container);
           
            // Initialize WebApi
            container.Register(AllTypes.FromAssembly(typeof(HomeController).Assembly)
                                        .BasedOn<ApiController>()
                                        .If(r => r.Name.EndsWith("Controller"))
                                        .LifestyleTransient());

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorCompositionRoot(container));
            
            GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(container);

            // Setup service locator
            var windsorServiceLocator = new WindsorServiceLocator(container);
            DomainEvents.ServiceLocator = windsorServiceLocator;
            ServiceLocator.SetLocatorProvider(() => windsorServiceLocator);
        }

        #endregion

    }
}