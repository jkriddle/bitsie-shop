using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using NHibernate.Transform;
using SharpArch.NHibernate;
using System.Collections.Generic;
namespace Bitsie.Shop.Infrastructure
{
    public class SubscriptionRepository : LinqRepository<Subscription>, ISubscriptionRepository
    {
        public IEnumerable<Subscription> Search(SubscriptionFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Subscription subscriptionAlias = null;

            var query = Session.QueryOver(() => subscriptionAlias);

            if (filter.ExpirationDate.HasValue)
            {
                query = query.Where(s => s.DateExpires.Date == filter.ExpirationDate.Value.Date);
            }

            if (filter.ExpiresBefore.HasValue)
            {
                query = query.Where(s => s.DateExpires.Date <= filter.ExpirationDate.Value.Date);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(s => s.Status == filter.Status.Value);
            }

            query.TransformUsing(Transformers.DistinctRootEntity);

            int firstResult = (page * numPerPage) - numPerPage;
            totalRecords = query.RowCount();

            // Sort
            if (filter.SortDirection == SortDirection.Ascending)
                query = query.OrderBy(Projections.Property(filter.SortColumn)).Asc;
            else
                query = query.OrderBy(Projections.Property(filter.SortColumn)).Desc;

            return query.Skip(firstResult).Take(numPerPage).List();
        }
    }
}
