using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class InvoiceItemRepository : LinqRepository<Domain.InvoiceItem>, IInvoiceItemRepository
    {
    }
}
