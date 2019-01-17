using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class ProductRepository : LinqRepository<Product>, IProductRepository
    {
        public IEnumerable<Product> Search(ProductFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Product productAlias = null;
            User userAlias = null;

            var query = Session.QueryOver(() => productAlias)
                .Left.JoinAlias(() => productAlias.User, () => userAlias);

            // Search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Product>(l => productAlias.Title).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            // User
            if (filter.Status.HasValue)
            {
                query.And(() => productAlias.Status == filter.Status.Value);
            }

            // User
            if (filter.UserId.HasValue)
            {
                query.And(() => userAlias.Id == filter.UserId.Value);
            }

            if (!String.IsNullOrEmpty(filter.MerchantId))
            {
                query.And(() => userAlias.MerchantId == filter.MerchantId);
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
