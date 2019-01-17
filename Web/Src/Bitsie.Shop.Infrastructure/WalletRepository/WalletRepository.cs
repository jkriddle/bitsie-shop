using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using SharpArch.NHibernate;

namespace Bitsie.Shop.Infrastructure
{
    public class WalletRepository : LinqRepository<Bip39Wallet>, IWalletRepository
    {
        /// <summary>
        ///  In order to determine next derivation without worrying about concurrent requests
        /// from other users (i.e. if two users make request at same time they could both receive the
        /// same derived address for payment) we use a stored procedure so SQL can place a lock on the record.
        /// </summary>
        /// <param name="walletId"></param>
        /// <returns></returns>
        public int GetNextDerivation(int walletId)
        {
            return Session.CreateSQLQuery("exec GetNextDerivation :walletId ")
                    .SetParameter("walletId", walletId)
                    .UniqueResult<int>();
        }
    }
}
