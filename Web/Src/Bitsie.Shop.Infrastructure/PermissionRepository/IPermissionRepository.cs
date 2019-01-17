using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitsie.Shop.Domain;
using SharpArch.Domain.PersistenceSupport;

namespace Bitsie.Shop.Infrastructure
{
    public interface IPermissionRepository : ILinqRepositoryWithTypedId<Permission, int>
    {
    }
}
