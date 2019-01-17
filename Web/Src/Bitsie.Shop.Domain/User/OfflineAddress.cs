using SharpArch.Domain.DomainModel;
using System.Linq;
using System.Collections.Generic;
using System;
using Bitsie.Shop.Domain.Bitcoin;

namespace Bitsie.Shop.Domain
{
    public class OfflineAddress : Entity
    {
        public virtual string Address { get; set; }
        public virtual string EncryptedPrivateKey { get; set; }
        public virtual string EmailNotifications { get; set; }
        public virtual string TextNotifications { get; set; }
        public virtual User User { get; set; }
        public virtual OfflineAddressStatus Status { get; set; }

        public virtual IList<string> EmailList
        {
            get
            {
                if (String.IsNullOrWhiteSpace(EmailNotifications)) return new List<string>();
                return EmailNotifications.Split(',').Where(r => !String.IsNullOrWhiteSpace(r)).ToList();
            }
        }

        public virtual IList<string> TextList
        {
            get
            {
                if (String.IsNullOrWhiteSpace(TextNotifications)) return new List<string>();
                return TextNotifications.Split(',').Where(r => !String.IsNullOrWhiteSpace(r)).ToList();
            }
        }

        public virtual KeyPair GetKeyPair(string salt)
        {
            string decrypted = Crypto.DecryptStringAES(EncryptedPrivateKey, salt);
            return new KeyPair(decrypted);
        }

    }
}
