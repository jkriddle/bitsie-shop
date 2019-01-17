using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Castle.Windsor;

namespace Bitsie.Shop.Web
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        protected WindsorContainer Container;

        public WindsorDependencyResolver(WindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            this.Container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return Container.Resolve(serviceType);
            }
            catch(Exception) {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Container.ResolveAll(serviceType).Cast<object>().ToList();
            } catch(Exception)
            {
                return new List<object>();
            }
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
    
}