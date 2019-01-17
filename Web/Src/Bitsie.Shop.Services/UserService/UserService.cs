using System;
using System.Collections.Generic;
using System.Linq;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using System.Text;
using Bitsie.Shop.Api;
using Bitsie.Shop.Domain.Bitcoin;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;
using System.Security.Cryptography;

namespace Bitsie.Shop.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly IConfigService _configService;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IAuthTokenRepository _authTokenRepository;
        private readonly INotificationService _notificationService;
        private readonly IWalletApi _walletApi;
        private readonly IBitcoinService _bitcoinService;
        private readonly IOfflineAddressRepository _offlineAddressRepository;
        private readonly ILogService _logService;
        private const int ResetPasswordTokenLength = 20;
        private readonly ISubscriptionRepository _subscriptionRepository;

        #endregion

        #region Constructor

        public UserService(IConfigService configService,
            IUserRepository userRepository, 
            IPermissionRepository permissionRepository,
            IAuthTokenRepository authTokenRepository,
            IWalletApi walletApi,
            ILogService logService,
            IBitcoinService bitcoinService,
            IOfflineAddressRepository offlineAddressRepository,
            INotificationService notificationService,
            ISubscriptionRepository subscriptionRepository)
        {
            _configService = configService;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _walletApi = walletApi;
            _bitcoinService = bitcoinService;
            _offlineAddressRepository = offlineAddressRepository;
            _authTokenRepository = authTokenRepository;
            _notificationService = notificationService;
            _logService = logService;
            _subscriptionRepository = subscriptionRepository;
        }

        #endregion

        #region CRUD Methods

        public void DeleteUser(User user)
        {
            user.Status = UserStatus.Suspended;
            _userRepository.Save(user);
        }

        /// <summary>
        /// Retrieve a single user by their unique ID.
        /// </summary>
        /// <param name="id">ID of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserById(int id)
        {
            return _userRepository.FindOne(id);
        }

        /// <summary>
        /// Retrieve a single user by their email.
        /// </summary>
        /// <param name="email">Email of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByEmail(string email)
        {
            return GetUserByEmail(email, null);
        }

        /// <summary>
        /// Retrieve a single user by their email.
        /// </summary>
        /// <param name="email">Email of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByEmail(string email, User merchant = null)
        {
            return _userRepository.FindAll().Where(u => u.Merchant == merchant).FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Retrieve a single user by their auth token.
        /// </summary>
        /// <param name="token">Token of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByToken(string token)
        {
            var user = _userRepository.FindAll().FirstOrDefault(u => u.AuthToken.Token == token);
            // Leaving this disabled so that mobile devices are always logged in
            //if (user != null && user.AuthToken.Expires <= DateTime.UtcNow) return null;
            return user;
        }

        /// <summary>
        /// Retrieve a single user by their merchant id
        /// </summary>
        /// <param name="id">Merchant ID of associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByMerchantId(string id)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.MerchantId == id);
        }

        /// <summary>
        /// Retrieve a single user by their reset token.
        /// </summary>
        /// <param name="token">Token for associated user</param>
        /// <returns>Associated user</returns>
        public User GetUserByResetToken(string token)
        {
            return _userRepository.FindAll().FirstOrDefault(u => u.ResetPasswordToken == token);
        }

        /// <summary>
        /// Retrieve a paged list of all users.
        /// </summary>
        /// <param name="filter">Filter to search users</param>
        /// <param name="currentPage">Current page number </param>
        /// <param name="numPerPage"># of records per page</param>
        /// <returns>Paged list of users</returns>
        public IPagedList<User> GetUsers(UserFilter filter, int currentPage, int numPerPage)
        {
            int totalRecords = 0;
            List<User> users = _userRepository.Search(filter, currentPage,
                numPerPage, out totalRecords).ToList();
            return new PagedList<User>(users, currentPage, numPerPage, totalRecords);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User to create</param>
        public void CreateUser(User user)
        {
            // Save permissions
            if (user.Role == Role.Administrator)
            {
                var p = _permissionRepository.FindAll().FirstOrDefault(pe => pe.Name == Permission.EditUsers);
                var up = new UserPermission()
                {
                    User = user,
                    Permission = p
                };
                user.Permissions.Add(up);

                p = _permissionRepository.FindAll().FirstOrDefault(pe => pe.Name == Permission.EditOrders);
                up = new UserPermission()
                {
                    User = user,
                    Permission = p
                };
                user.Permissions.Add(up);

                p = _permissionRepository.FindAll().FirstOrDefault(pe => pe.Name == Permission.EditProducts);
                up = new UserPermission()
                {
                    User = user,
                    Permission = p
                };
                user.Permissions.Add(up);
            }

            if (String.IsNullOrEmpty(user.MerchantId)) user.MerchantId = CreateGuid(6);

            // Save user
            _userRepository.Save(user);

            // If user is a tipsie account, create an offline address
            if (user.Role == Role.Tipsie)
            {
                var kp = KeyPair.Create(user.Email);

                var address = new OfflineAddress
                {
                    EmailNotifications = user.Email,
                    Address = kp.AddressBase58,
                    Status = OfflineAddressStatus.Active,
                    EncryptedPrivateKey = Crypto.EncryptStringAES(kp.PrivateKeyBase58,  System.Text.Encoding.Default.GetString(user.HashedPassword))
                };
                SaveOfflineAddress(user, address);
                try
                {
                    _bitcoinService.CreateWebhook(address.Address, 0);
                }
                catch (Exception ex)
                {
                    // Fail silently
                    // TODO: check for 400 error (duplicate address sent)
                }
                user.OfflineAddresses.Add(address);
            }

            // Send notification
            _notificationService.Notify(_configService.AppSettings("NotificationEmail"), 
                "A new user has signed up for Bitsie Shop!",
                "SignupAdmin", user);

        }

        /// <summary>
        /// Update a user's status
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newStatus"></param>
        public void UpdateStatus(User user, UserStatus newStatus)
        {
            // Notifications
            if (newStatus != user.Status)
            {
                if (newStatus == UserStatus.AwaitingApproval
                    && user.Status == UserStatus.Pending)
                {
                    // User has signed up but not approved yet
                    _notificationService.Notify(user.Email, "Welcome to Bitsie Shop", "Welcome", null);

                    // Admin
                    _notificationService.Notify(_configService.AppSettings("NotificationEmail"), 
                        "Bitsie Shop Account Awaiting Approval: " + user.Email, 
                        "AwaitingApproval", user);
                }

                if (newStatus == UserStatus.Active
                    && user.Status == UserStatus.AwaitingApproval)
                {
                    // User has been approved
                    _notificationService.Notify(user.Email, "Your Bitsie Shop Account Has Been Approved", "Approved", null);
                }
            }

            user.Status = newStatus;
            UpdateUser(user);
        }

        /// <summary>
        /// Retrieve active offline addresses for a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<OfflineAddress> GetOfflineAddressesByUserId(int id)
        {
            return _offlineAddressRepository.FindAll().Where(u => u.User.Id == id && u.Status == OfflineAddressStatus.Active).ToList();
        }

        public void DeleteOfflineAddress(OfflineAddress address)
        {
            address.Status = OfflineAddressStatus.Deleted;
            _offlineAddressRepository.Save(address);
        }

        public void SaveOfflineAddress(User user, OfflineAddress address)
        {
            // Generate new
            address.User = user;
            if (String.IsNullOrEmpty(address.Address))
            {
                address.Address = _walletApi.CreateAddress();
                string hookId = _bitcoinService.CreateWebhook(address.Address, 0);

                _logService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    Message = "Created offline address " + address.Address + ", webhook ID " + hookId,
                    LogDate = DateTime.UtcNow,
                    Level = LogLevel.Info,
                    Details = "",
                    User = user
                });
            }

            _offlineAddressRepository.Save(address);
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        /// <param name="user">User to update</param>
        /// <param name="newPassword">New password for user</param>
        public void UpdateUser(User user, string newPassword = null)
        {
            var existing = GetUserById(user.Id);
            if (!string.IsNullOrEmpty(newPassword))
            {
                GenerateUserPassword(user, newPassword);
            }

            // Create any new addresses (updated ones are saved by user repository method
            foreach (var a in user.OfflineAddresses)
            {
                if (String.IsNullOrEmpty(a.Address)) SaveOfflineAddress(user, a); 
            }

            _userRepository.Save(user);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check that user data is valid.
        /// </summary>
        /// <param name="user">User to validate</param>
        /// <param name="validationDictionary">Dictionary of errors</param>
        /// <returns></returns>
        public bool ValidateUser(User user, IValidationDictionary validationDictionary)
        {
            User existingUser = null;
            if (user.Id != 0) existingUser = GetUserById(user.Id);

            // Email cannot be null
            if (string.IsNullOrEmpty(user.Email))
            {
                validationDictionary.AddError("Email", "Email is required.");
            } else if (!EmailValidator.IsValid(user.Email))
            {
                validationDictionary.AddError("Email", "Invalid email address.");
            } else {
                // Check for existing email either system-wide or for a specific merchant
                var existing = _userRepository.FindAll().Where(u => u.Email == user.Email && u.Merchant == user.Merchant);

                if (existing.Count() > 0 && existing.First().Id != user.Id)
                {
                    validationDictionary.AddError("Email", "Email is already in use. Please try another");
                }
            }

            if (user.HashedPassword == null || user.HashedPassword.Length == 0)
            {
                validationDictionary.AddError("Password", "Password is required.");
            }

            if (user.Settings.PaymentMethod == PaymentMethod.Bitcoin
                && String.IsNullOrEmpty(user.Settings.PaymentAddress))
            {
                validationDictionary.AddError("PaymentAddress", "Bitcoin payment address is required.");
            }

            if (!String.IsNullOrEmpty(user.Settings.FreshbooksApiUrl) &&
                String.IsNullOrEmpty(user.Settings.FreshbooksAuthToken))
            {
                validationDictionary.AddError("FreshbooksAccountName", "Freshbooks auth token is required.");
            }

            if (!String.IsNullOrEmpty(user.Settings.FreshbooksAuthToken) &&
                String.IsNullOrEmpty(user.Settings.FreshbooksApiUrl))
            {
                validationDictionary.AddError("FreshbooksApiUrl", "Freshbooks API Url is required.");
            }

            if (user.Role == Role.Merchant)
            {
                if (String.IsNullOrEmpty(user.MerchantId))
                {
                    validationDictionary.AddError("MerchantId", "Hosted checkout URL is invalid.");
                }
                else
                {
                    var valid = Regex.IsMatch(user.MerchantId, "^[a-zA-Z0-9_\\-]*$");

                    // Validate length
                    if (user.MerchantId.Length >= 50)
                    {
                        validationDictionary.AddError("MerchantId", "Max checkout URL length is 50 characters.");
                    }
                    else if (!valid)
                    {
                        // Validate characters
                        validationDictionary.AddError("MerchantId", "Invalid hosted checkout URL. Only numbers, letters, hyphens and underscores are allowed.");
                    }
                    else
                    {
                        // Validate conflicts
                        var merchantConflict = _userRepository.FindAll().FirstOrDefault(u => u.MerchantId == user.MerchantId);
                        if (merchantConflict != null && merchantConflict.Id != user.Id)
                        {
                            validationDictionary.AddError("MerchantId", "Hosted checkout URL is already in use. Please try another.");
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(user.Settings.HtmlTemplate))
            {
                if (!user.Settings.HtmlTemplate.Contains("{{form}}"))
                {
                    validationDictionary.AddError("HtmlTemplate",
                        "HTML template must contain the text \"{{form}}}\" where you would like to embed Bitsie Shop forms.");
                }
                else
                {
                    user.Settings.HtmlTemplate = HtmlHelper.Sanitize(user.Settings.HtmlTemplate); 
                }
            }

            foreach(var a in user.OfflineAddresses) {

                // Validate emails
                    IList<string> records = new List<string>();
                string emails = String.IsNullOrEmpty(a.EmailNotifications) ? "" : a.EmailNotifications;
                if (emails.Contains(",")) records = emails.Split(',').ToList();
                else records.Add(emails);

                for(var i = 0; i < records.Count; i++) {
                    if (!String.IsNullOrEmpty(records[i]) && !EmailValidator.IsValid(records[i])) {
                        validationDictionary.AddError("Email" + i, "Invalid offline address email: " + records[i]);
                    }
                }

                // Validate phone numbers
                records = new List<string>();
                string sms = String.IsNullOrEmpty(a.TextNotifications) ? "" : a.TextNotifications;
                if (sms.Contains(",")) records = sms.Split(',').ToList();
                else records.Add(sms);

                for(var i = 0; i < records.Count; i++)
                {
                    if (!String.IsNullOrEmpty(records[i]) && records[i].Length != 10)
                    {
                        validationDictionary.AddError("Phone" + i, "Invalid offline address phone number: " + records[i]);
                    }
                }
            }

            return validationDictionary.IsValid;
        }

        #endregion

        #region Authentication and Passwords

        /// <summary>
        /// Remove auth token to prevent further authenticated requests
        /// </summary>
        /// <param name="token"></param>
        public void DestroyAuthToken(User user)
        {
            if (user.AuthToken != null)
            {
                var token = _authTokenRepository.FindOne(user.AuthToken.Id);
                user.AuthToken = null;
                _authTokenRepository.Delete(token);
            }
        }

        /// <summary>
        /// Check credentials for a user's token
        /// </summary>
        /// <param name="token">Token to authenticate</param>
        /// <returns>True if user was successfully logged in.</returns>
        public User Authenticate(string token)
        {
            var user = GetUserByToken(token);
            if (user == null) return null;

            // Do not allow deleted users to authenticate
            if (user.Status == UserStatus.Suspended) return null;

            return user;
        }

        /// <summary>
        /// Check credentials and log a user into the system.
        /// </summary>
        /// <param name="email">Email to authenticate</param>
        /// <param name="password">Password to authenticate</param>
        /// <returns>True if user was successfully logged in.</returns>
        public User Authenticate(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null) return null;
            var hashedPassword = HashSalt.HashPassword(password, user.Salt);
            if (!ByteArraysEqual(user.HashedPassword, hashedPassword)) return null;

            // Do not allow deleted users to authenticate
            if (user.Status == UserStatus.Suspended) return null;

            if (user.AuthToken != null)
            {
                // Extend existing auth token
                user.AuthToken.Expires = DateTime.UtcNow.AddDays(30);
            }
            else
            {
                // Create new auth token
                user.AuthToken = new AuthToken
                {
                    //Token = Convert.ToBase64String(HashSalt.GenerateSalt()),
                    Token = GenerateAuthToken(),
                    Expires = DateTime.UtcNow.AddDays(30)
                };
            }
            _authTokenRepository.Save(user.AuthToken);

            _userRepository.Save(user);
            return user;
        }

        private string GenerateAuthToken()
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[32];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(32);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Generate an encrypted password for the specified user.
        /// </summary>
        /// <param name="user">User to generate password for</param>
        /// <param name="plainTextPassword">Password to encrypt.</param>
        public void GenerateUserPassword(User user, string plainTextPassword)
        {
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(plainTextPassword, user.Salt);
        }

        /// <summary>
        /// Generate a password reset token and email the link to the user. 
        /// </summary>
        /// <param name="user"></param>
        public void GenerateResetRequest(User user)
        {
            user.ResetPasswordToken = KeyGenerator.GetUniqueKey(ResetPasswordTokenLength);
            _userRepository.Save(user);

            _notificationService.Notify(user.Email, 
                "Your password reset link for Bitsie Shop", 
                "ForgotPassword", new
                {
                    ResetToken = user.ResetPasswordToken
                });
        }

        /// <summary>
        /// Update a user's password and send notification email.
        /// </summary>
        /// <param name="user">User to reset password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if reset token was found</returns>
        public void ResetPassword(User user, string newPassword)
        {
            if (String.IsNullOrEmpty(newPassword))
            {
                throw new Exception("New password cannot be empty.");
            }

            // Update password
            user.Salt = HashSalt.GenerateSalt();
            user.HashedPassword = HashSalt.HashPassword(newPassword, user.Salt);
            user.ResetPasswordToken = null;
            _userRepository.SaveAndEvict(user);

            // Send notification email
            _notificationService.Notify(user.Email, "Your Bitsie Shop password has been reset", "PasswordReset", null);
        }

        /// <summary>
        /// Check that two bytes are equal (for encrypted password comparison).
        /// </summary>
        /// <param name="b1">First byte</param>
        /// <param name="b2">Byte to compare</param>
        /// <returns>True if bytes are equal</returns>
        private bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            return !b1.Where((t, i) => t != b2[i]).Any();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generate a randomized string to be used for merchant ID/URLs
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string CreateGuid(int size)
        {
            return GuidHelper.Create(size);
        }

        #endregion
    }
}
