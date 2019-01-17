using SharpArch.Domain.DomainModel;
using System.Collections.Generic;

namespace Bitsie.Shop.Domain
{
    public class Bip39Wallet : Entity
    {
        public virtual string PublicMasterKey { get; set; }
        public virtual string EncryptedPrivateMasterKey { get; set; }
        public virtual IList<WalletAddress> Addresses { get; set; }
        public virtual User User { get; set; }
        public virtual int? LastDerivation { get; set; }
    }
}
