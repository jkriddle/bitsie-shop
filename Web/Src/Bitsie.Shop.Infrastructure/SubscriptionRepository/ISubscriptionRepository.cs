using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;
using System.Collections.Generic;

namespace Bitsie.Shop.Infrastructure
{
    public interface ISubscriptionRepository : ILinqRepositoryWithTypedId<Subscription, int>
    {
        IEnumerable<Subscription> Search(SubscriptionFilter filter, int page, int numPerPage, out int totalRecords);
    }
}
