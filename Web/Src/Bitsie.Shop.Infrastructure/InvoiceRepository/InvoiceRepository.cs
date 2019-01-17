using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class InvoiceRepository : LinqRepository<Domain.Invoice>, IInvoiceRepository
    {
        /// <summary>
        /// Search invoices
        /// </summary>
        /// <param name="filter">Parameters to filter invoices by</param>
        /// <param name="page">Current page</param>
        /// <param name="numPerPage">Number of records per page</param>
        /// <param name="totalRecords">Total records ignoring paging</param>
        /// <returns></returns>
        public IEnumerable<Domain.Invoice> Search(InvoiceFilter filter, int page, int numPerPage, out int totalRecords)
        {
            Domain.Invoice invoiceAlias = null;
            Domain.User customerAlias = null;

            var query = Session.QueryOver(() => invoiceAlias)
                .Left.JoinAlias(() => invoiceAlias.Customer, () => customerAlias);

            // Search term
            if (!String.IsNullOrEmpty(filter.SearchTerm))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Invoice>(l => invoiceAlias.InvoiceNumber).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                or.Add(Restrictions.On<Invoice>(l => invoiceAlias.InvoiceDescription).IsLike(filter.SearchTerm, MatchMode.Anywhere));
                query.And(or);
            }

            if (!String.IsNullOrEmpty(filter.CustomerLastName))
            {
                var or = Restrictions.Disjunction();
                or.Add(Restrictions.On<Invoice>(l => customerAlias.LastName).IsLike(filter.CustomerLastName, MatchMode.Anywhere));
                query.And(or);
            }

            if (filter.InvoiceId.HasValue)
            {
                query.And(t => t.Id == filter.InvoiceId.Value);
            }

            if (filter.Status.HasValue)
            {
                query.And(t => t.Status == filter.Status.Value);
            }

            if (filter.CustomerId.HasValue)
            {
                query.And(t => t.Customer.Id == filter.CustomerId.Value);
            }

            if (filter.MerchantId.HasValue)
            {
                query.And(t => t.Merchant.Id == filter.MerchantId.Value);
            }

            if (filter.SendDate.HasValue){
                query.And(t => t.SendDate >= filter.SendDate.Value);
            }

            if (filter.DueDate.HasValue)
            {
                query.And(t => t.DueDate >= filter.DueDate.Value);
            }
            
            if (filter.StartDate.HasValue)
            {
                query.And(t => t.DueDate >= filter.StartDate.Value);
            }

            if (filter.EndDate.HasValue)
            {
                query.And(t => t.DueDate < filter.EndDate.Value.AddDays(1));
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
