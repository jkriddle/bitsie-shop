using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class OrderRepository : LinqRepository<Domain.Order>, IOrderRepository
    {
        /// <summary>
        /// Search orders
        /// </summary>
        /// <param name="filter">Parameters to filter orders by</param>
        /// <param name="page">Current page</param>
        /// <param name="numPerPage">Number of records per page</param>
        /// <param name="totalRecords">Total records ignoring paging</param>
        /// <returns></returns>
        public IEnumerable<Domain.Order> Search(OrderFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Domain.Order orderAlias = null;
            Domain.User userAlias = null;
            Domain.Settings settingsAlias = null;
            Domain.Product productAlias = null;

            var query = Session.QueryOver(() => orderAlias)
                .Left.JoinAlias(() => orderAlias.User, () => userAlias)
                .Left.JoinAlias(() => userAlias.Settings, () => settingsAlias)
                .Left.JoinAlias(() => orderAlias.Product, () => productAlias);

            // Search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Log>(l => orderAlias.OrderNumber).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<Log>(l => orderAlias.PaymentAddress).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<Log>(l => orderAlias.Description).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            if (filter.UserId.HasValue && filter.OnlyChildren.HasValue && filter.OnlyChildren.Value)
            {
                // Only retrieve orders from child users of this merchant
                query.And(t => userAlias.Merchant.Id == filter.UserId.Value);
            }
            else if (filter.UserId.HasValue && filter.IncludeChildren.HasValue && filter.IncludeChildren.Value)
            {
                // Retrieve this user and all child account orders
                query.And(t => userAlias.Merchant.Id == filter.UserId.Value || userAlias.Id == filter.UserId.Value);
            } else if (filter.UserId.HasValue)
            {
                // Just this user's orders
                query.And(t => t.User.Id == filter.UserId.Value);
            }

            if (filter.Status.HasValue)
            {
                query.And(t => t.Status == filter.Status.Value);
            }
            else
            {
                // By default only get the visible statuses
                query.And(t => t.Status == OrderStatus.Complete 
                            || t.Status == OrderStatus.Paid
                            || t.Status == OrderStatus.Confirmed
                            || t.Status == OrderStatus.Partial 
                            || t.Status == OrderStatus.Refunded);
            }
            if (filter.PaymentMethod.HasValue)
            {
                query.And(t => settingsAlias.PaymentMethod == filter.PaymentMethod.Value);
            }

            if (filter.StartDate.HasValue)
            {
                query.And(t => t.OrderDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query.And(t => t.OrderDate < filter.EndDate.Value.AddDays(1));
            }

            if (filter.AfterId.HasValue)
            {
                query.And(t => t.Id > filter.AfterId.Value);
            }

            if (filter.OrderId.HasValue)
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Domain.Order>(u => orderAlias.Id).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
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
