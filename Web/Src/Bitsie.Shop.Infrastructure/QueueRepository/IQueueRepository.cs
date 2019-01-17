using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace Bitsie.Shop.Infrastructure
{
    public interface IQueueRepository : ILinqRepositoryWithTypedId<Queue, int>
    {
    }
}
