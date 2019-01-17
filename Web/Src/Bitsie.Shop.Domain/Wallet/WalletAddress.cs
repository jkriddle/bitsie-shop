using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class WalletAddress : Entity
    {
        public virtual Bip39Wallet Wallet { get; set; }
        public virtual string Address { get; set; }
        public virtual DateTime DateExpires { get; set; }
        public virtual bool IsUsed { get; set; }
        public virtual int Derivation { get; set; }
    }
}
