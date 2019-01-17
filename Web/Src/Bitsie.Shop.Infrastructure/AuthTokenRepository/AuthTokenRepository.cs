using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class AuthTokenRepository : LinqRepository<AuthToken>, IAuthTokenRepository
    {
        public override void Delete(AuthToken token)
        {
            Session.Delete(token);
            SessionFactory.Instance.GetCurrentSession().Transaction.Commit();
            Session.Flush();
        }
    }
}
