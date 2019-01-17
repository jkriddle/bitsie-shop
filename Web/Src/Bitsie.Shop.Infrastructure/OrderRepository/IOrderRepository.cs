using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using SharpArch.Domain.PersistenceSupport;

namespace Bitsie.Shop.Infrastructure
{
    public interface IOrderRepository : ILinqRepositoryWithTypedId<Order, int>
    {
        IEnumerable<Order> Search(OrderFilter filter, int page, int numPerPage, out int totalRecords);
    }
}
