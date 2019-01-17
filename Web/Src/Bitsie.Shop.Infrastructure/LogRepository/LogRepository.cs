using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class LogRepository : LinqRepository<Log>, ILogRepository
    {
        public IEnumerable<Log> Search(LogFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Log logAlias = null;
            User userAlias = null;

            var query = Session.QueryOver(() => logAlias)
                .Left.JoinAlias(() => logAlias.User, () => userAlias);

            // Search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Log>(l => logAlias.Details).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<Log>(l => logAlias.Message).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<Log>(l => logAlias.IpAddress).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            // Details
            if (!String.IsNullOrEmpty(filter.Details))
            {
                query.And(Restrictions.On<Log>(l => logAlias.Details).IsLike(filter.Details, MatchMode.Anywhere));
            }

            // Log Category
            if (filter.LogCategory.HasValue)
            {
                query.And(() => logAlias.Category == filter.LogCategory.Value);
            }

            // Log Date
            if (filter.StartDate.HasValue)
            {
                query.And(() => logAlias.LogDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query.And(() => logAlias.LogDate < filter.EndDate.Value.AddDays(1));
            }

            // Log Level
            if (filter.LogLevel.HasValue)
            {
                query.And(() => logAlias.Level == filter.LogLevel.Value);
            }

            // User
            if (filter.UserId.HasValue)
            {
                query.And(() => userAlias.Id == filter.UserId.Value);
            }

            // Message
            if (!String.IsNullOrEmpty(filter.Message))
            {
                query.And(Restrictions.On<Log>(l => logAlias.Message).IsLike(filter.Message, MatchMode.Anywhere));
            }

            // Details
            if (!String.IsNullOrEmpty(filter.Details))
            {
                query.And(Restrictions.On<Log>(l => logAlias.Details).IsLike(filter.Details, MatchMode.Anywhere));
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
