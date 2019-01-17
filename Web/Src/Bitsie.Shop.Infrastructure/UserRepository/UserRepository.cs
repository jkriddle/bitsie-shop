using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class UserRepository : LinqRepository<User>, IUserRepository
    {

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="filter">Parameters to filter users by</param>
        /// <param name="page">Current page</param>
        /// <param name="numPerPage">Number of records per page</param>
        /// <param name="totalRecords">Total records ignoring paging</param>
        /// <returns></returns>
        public IEnumerable<User> Search(UserFilter filter, int page, int numPerPage, out int totalRecords)
        {
            User userAlias = null;
            Settings settingsAlias = null;
            Company companyAlias = null;

            var query = Session.QueryOver(() => userAlias)
                                            .Inner.JoinAlias(() => userAlias.Settings, () => settingsAlias)
                                            .Inner.JoinAlias(() => userAlias.Company, () => companyAlias);

            // Filter search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<User>(u => userAlias.Email).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => userAlias.FirstName).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<User>(u => userAlias.LastName).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            // Filter email
            if (!String.IsNullOrEmpty(filter.Email))
            {
                query.And(Restrictions.On<User>(u => userAlias.Email).IsLike(filter.Email, MatchMode.Anywhere));
            }

            // Filter by merchant
            if (filter.Merchant != null)
            {
                query.And(u => userAlias.Merchant == filter.Merchant);
            }

            // Filter role
            if (filter.Role.HasValue)
            {
                query.And(u => userAlias.Role == filter.Role.Value);
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
