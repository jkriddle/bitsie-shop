using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class PermissionRepository : LinqRepository<Permission>, IPermissionRepository
    {
        
    }
}
