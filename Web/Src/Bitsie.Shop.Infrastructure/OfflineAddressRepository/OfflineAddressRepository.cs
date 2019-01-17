using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class OfflineAddressRepository : LinqRepository<OfflineAddress>, IOfflineAddressRepository
    {
    }
}
