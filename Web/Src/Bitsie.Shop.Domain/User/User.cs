using System.Collections.Generic;
using System.Linq;
using SharpArch.Domain.DomainModel;
using System;

namespace Bitsie.Shop.Domain
{
    public class User : Entity
    {

        #region Constructor

        public User()
        {
            Status = UserStatus.Active;
            Permissions = new List<UserPermission>();
            Logs = new List<Log>();
            Settings = new Settings();
            OfflineAddresses = new List<OfflineAddress>();
            DateRegistered = DateTime.UtcNow;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Unique slug to identify merchants (used for URLs)
        /// </summary>
        public virtual string MerchantId { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// User's email address
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// User's one-way encrypted password hash
        /// </summary>
        public virtual byte[] HashedPassword { get; set; }

        /// <summary>
        /// Salt used for encryption
        /// </summary>
        public virtual byte[] Salt { get; set; }

        /// <summary>
        /// Authorization token for API login
        /// </summary>
        public virtual AuthToken AuthToken { get; set; }

        /// <summary>
        /// User's role
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// User's status
        /// </summary>
        public virtual UserStatus Status { get; set; }

        /// <summary>
        /// Granular permissions assigned to user
        /// </summary>
        public virtual IList<UserPermission> Permissions { get; set; }

        /// <summary>
        /// Logs attributed to this user
        /// </summary>
        public virtual IList<Log> Logs { get; set; }

        /// <summary>
        /// Unique string used to reset user's password
        /// </summary>
        public virtual string ResetPasswordToken { get; set; }

        /// <summary>
        /// User application settings
        /// </summary>
        public virtual Settings Settings { get; set; }

        /// <summary>
        /// Company information
        /// </summary>
        public virtual Company Company { get; set; }

        /// <summary>
        /// Subscription information
        /// </summary>
        public virtual Subscription Subscription { get; set; }

        /// <summary>
        /// Associated merchant (for customer and tipsie accounts)
        /// </summary>
        public virtual User Merchant { get; set; }

        /// <summary>
        /// Offline addresses for this user's account
        /// </summary>
        public virtual IList<OfflineAddress> OfflineAddresses { get; set; }

        /// <summary>
        /// Date user signed up
        /// </summary>
        public virtual DateTime DateRegistered { get; set; }

        /// <summary>
        /// Bitcoin wallet addresses
        /// </summary>
        public virtual Bip39Wallet Wallet { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Indictates if this user has permission to the specified
        /// action.
        /// </summary>
        /// <param name="permission">Permission to check</param>
        /// <returns>If user has permission to specified action</returns>
        public virtual bool HasPermission(string permission)
        {
            return Permissions.Any(p => p.Permission.Name == permission);
        }

        #endregion

    }
}