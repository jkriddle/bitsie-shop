using SharpArch.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public class Transaction : Entity
    {
        public virtual DateTime TransactionDate { get; set; }
        public virtual string TxId { get; set; }
        public virtual int? Block { get; set; }
        public virtual string Address { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual Order Order { get; set; }
        public virtual TransactionCategory Category { get; set; }
    }
}
