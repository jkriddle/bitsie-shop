using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class Payout : Entity
    {

        #region Constructor

        public Payout()
        {
        }

        #endregion

        #region Public Properties

        public virtual DateTime DateProcessed { get; set; }
        public virtual string ConfirmationNumber { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual decimal PayoutAmount { get; set; }
        public virtual decimal PayoutFee { get; set; }
        public virtual IList<Order> Order { get; set; }
        #endregion
    }
}