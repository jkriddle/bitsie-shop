using Bitsie.Shop.Infrastructure;
using NHibernate;
using NHibernate.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Bitsie.Shop.Web.Api.Attributes
{
    public class NhSessionManagementAttribute : ActionFilterAttribute
    {
        public NhSessionManagementAttribute()
        {
            SessionFactory = Bitsie.Shop.Infrastructure.SessionFactory.Instance;
        }

        private ISessionFactory SessionFactory { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var session = SessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            session.BeginTransaction();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var session = SessionFactory.GetCurrentSession();
            var transaction = session.Transaction;
            if (transaction != null && transaction.IsActive)
            {
                transaction.Commit();
            }
            session = CurrentSessionContext.Unbind(SessionFactory);
            session.Close();
        }
    }

}