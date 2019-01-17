using System;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Services;
using Bitsie.Shop.Web.Api.Models;
using Bitsie.Shop.Web.Api.Attributes;
using Bitsie.Shop.Web.Api.Providers;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class UserController : BaseApiController
    {
        #region Fields

        private readonly IAuth _auth;
        private readonly IMapperService _mapper;
        private readonly IConfigService _configService;
        ISubscriptionService _subscriptionService;

        #endregion

        #region Constructor

        public UserController(IAuth auth, 
            IUserService userService,
            ILogService logService,
            IConfigService configService,
            ISubscriptionService subscriptionService,
            IMapperService mapper) : base(userService, logService)
        {
            _auth = auth;
            _mapper = mapper;
            _configService = configService;
            _subscriptionService = subscriptionService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a single user by ID
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>User data</returns>
        [HttpGet, RequiresApiAuth]
        public AdminUserViewModel GetSecure(int? id)
        {
            var user = id.HasValue ? UserService.GetUserById(id.Value) : CurrentUser;
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers)
                && user.Id != CurrentUser.Id)
            {
                //if this isn't a tipsie role and it's not connected to the current users merchant account
                if(user.Role != Role.Tipsie && user.MerchantId != CurrentUser.MerchantId)
                {
                    throw new HttpException(401, "You do not have permissions to complete this action.");
                }
                
            }

            return new AdminUserViewModel(user);
        }

        /// <summary>
        /// Retrieve a single user by ID
        /// </summary>
        /// <param name="id">User's ID</param>
        /// <returns>User data</returns>
        [HttpGet, RequiresApiAuth]
        public UserViewModel GetOne(int? id)
        {
            var user = id.HasValue ? UserService.GetUserById(id.Value) : CurrentUser;
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers)
                && user.Id != CurrentUser.Id)
            {
                if (user.Merchant.Id != CurrentUser.Id)
                {
                    throw new HttpException(401, "You do not have permissions to complete this action.");
                }
            }

            return new UserViewModel(user);
        }

        /// <summary>
        /// Retrieve a list of users based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of users</returns>
        [HttpGet, RequiresApiAuth, ApiRequiresRole(Role.Administrator, Role.Merchant)]
        public UserListViewModel Get([FromUri]UserListInputModel inputModel)
        {
            if (inputModel == null) inputModel = new UserListInputModel();

            var filter = new UserFilter();
            _mapper.Map(inputModel, filter);

            // Only return a merchant's own customers
            if (CurrentUser.Role == Role.Merchant)
            {
                filter.Merchant = CurrentUser;
            }

            var users = UserService.GetUsers(filter, inputModel.CurrentPage, inputModel.NumPerPage);

            if (inputModel.Export)
            {
                Export("Bitsie_Users", users.AllItems);
                return null;
            }

            return new UserListViewModel(users);
        }

        /// <summary>
        /// Mark user as deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, RequiresApiAuth, ApiRequiresRole(Role.Administrator)]
        public BaseResponseModel Delete(int id)
        {
            var vm = new BaseResponseModel();

            // Get existing user
            var user = UserService.GetUserById(id);
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Check permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers))
            {
                throw new HttpException(401, "You do not have permissions to complete this action.");
            }

            UserService.DeleteUser(user);

            LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    User = CurrentUser,
                    Level = LogLevel.Info,
                    Message = "User " + user.Email + " (ID #" + user.Id + ") was deleted."
                });

            return new BaseResponseModel
                {
                    Success = true
                };
        }

        /// <summary>
        /// Generates a new token that will allow the user to update their password
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel ForgotPassword(ForgotPasswordInputModel inputModel)
        {
            // Get existing user
            var vm = new BaseResponseModel();
            var user = UserService.GetUserByEmail(inputModel.Email);
            if (user != null)
            {
                UserService.GenerateResetRequest(user);
                vm.Success = true;
            }

            return vm;
        }

        /// <summary>
        /// Updates a user's current password to the specified new password
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponseModel ResetPassword(ResetPasswordInputModel inputModel)
        {
            // Get existing user
            var vm = new BaseResponseModel();
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            if (validationState.IsValid)
            {
                var user = UserService.GetUserByResetToken(inputModel.ResetToken);
                if (user != null)
                {
                    UserService.ResetPassword(user, inputModel.Password);
                    vm.Success = true;
                }
                else
                {
                    validationState.AddError("ResetToken", "Invalid reset token.");
                }
            }

            vm.Errors = validationState.Errors;

            return vm;
        }


        /// <summary>
        /// Customer/Tipsie Signup
        /// </summary>
        /// <param name="inputModel">Sign up parameters</param>
        /// <returns>Sign up success/failure information</returns>
        [HttpPost]
        public BaseResponseModel SignUp(SignUpInputModel inputModel)
        {
            var vm = new AuthResponseModel();
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            string merchantId = null;
            Role role = Role.Tipsie;
            string password = inputModel.Password;
            User merchant = null;

            if (CurrentUser != null && CurrentUser.Role == Role.Merchant)
            {
                role = inputModel.IsTipsieUser ? Role.Tipsie : Role.Customer;
                password = UserService.CreateGuid(10); // generate random password
                merchant = CurrentUser;
            }
            else
            {
                merchantId = UserService.CreateGuid(6);
            }

            var user = new User
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Email = inputModel.Email,
                Role = role,
                Status = UserStatus.Active,
                MerchantId = merchantId,
                Merchant = merchant,
                Company = new Company()
            };

            UserService.GenerateUserPassword(user, password);

            if (UserService.ValidateUser(user, validationState))
            {
                UserService.CreateUser(user);

                // Update mobile number for tipsie address and log in
                if (user.Role == Role.Tipsie)
                {
                    var address = user.OfflineAddresses.First();
                    address.TextNotifications = inputModel.Phone;
                    UserService.SaveOfflineAddress(user, address);

                    if (CurrentUser == null || CurrentUser.Role != Role.Merchant)
                    {
                        // Authenticate to create token
                        user = UserService.Authenticate(user.Email, inputModel.Password);
                        _auth.DoAuth(user.Id.ToString(), false);
                        vm.Token = user.AuthToken.Token;
                        vm.Expires = user.AuthToken.Expires;
                    }
                }

                vm.User = new UserViewModel(user);
                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        /// <summary>
        /// Alias for /user/signup for API implementation
        /// </summary>
        /// <param name="inputModel">Sign up parameters</param>
        /// <returns>Sign up success/failure information</returns>
        [HttpPost]
        public BaseResponseModel Create(SignUpInputModel inputModel)
        {
            return SignUp(inputModel);
        }


        /// <summary>
        /// Sign Up Step 1
        /// </summary>
        /// <param name="inputModel">Sign in parameters</param>
        /// <returns>Sign in success/failure information</returns>
        [HttpPost]
        public BaseResponseModel SignUpStart(SignUpInputModel inputModel)
        {
            var vm = new AuthResponseModel();
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            var user = new User
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                Email = inputModel.Email,
                Role = Role.Merchant,
                Status = UserStatus.Active,
                MerchantId = UserService.CreateGuid(6),
                Company = new Company()
            };

            UserService.GenerateUserPassword(user, inputModel.Password);

            if (UserService.ValidateUser(user, validationState))
            {
                UserService.CreateUser(user);

                // Authenticate to create token
                user = UserService.Authenticate(user.Email, inputModel.Password);
                _auth.DoAuth(user.Id.ToString(), false);

                vm.User = new UserViewModel(user);
                vm.Success = true;
                vm.Token = user.AuthToken.Token;
                vm.Expires = user.AuthToken.Expires;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        /// <summary>
        /// Attempt to sign a user into the system
        /// </summary>
        /// <param name="inputModel">Sign in parameters</param>
        /// <returns>Sign in success/failure information</returns>
        [HttpPost]
        public BaseResponseModel SignIn(SignInInputModel inputModel)
        {
            var vm = new AuthResponseModel();

            string agent = HttpContext.Current.Request.UserAgent;
            if (inputModel.hashcashid == null && agent != "android")
            {
                vm.Errors.Add("AuthFail2");
                return vm;
            }

            if (agent != "android")
            {

                Uri hashcheckuri = new Uri("https://hashcash.io/api/checkwork/" + inputModel.hashcashid + "?apikey=PRIVATE-ca98f914-ee51-4e49-99ed-3eb598e2bff1");
                HttpWebRequest requestFile = (HttpWebRequest)WebRequest.Create(hashcheckuri);
                requestFile.ContentType = "application/html";
                HttpWebResponse webResp = requestFile.GetResponse() as HttpWebResponse;

                if (requestFile.HaveResponse && agent != null)
                {
                    if (webResp.StatusCode == HttpStatusCode.OK || webResp.StatusCode == HttpStatusCode.Accepted)
                    {
                        StreamReader respReader = new StreamReader(webResp.GetResponseStream());
                        dynamic respStr = JObject.Parse(respReader.ReadToEnd());
                        if (respStr == null)
                        {
                            vm.Errors.Add("AuthFail3");
                            return vm;
                        }
                        if (respStr.totalDone < 0.001)
                        {
                            vm.Errors.Add("AuthFail3");
                            return vm;
                        }
                    }
                }

            }

            var user = UserService.Authenticate(inputModel.Email, inputModel.Password);
            if (user != null)
            {
                if (user.Status != UserStatus.Active && user.Status != UserStatus.Suspended)
                {
                    vm.Errors.Add("AuthFail4.");
                    return vm;
                }
                _auth.DoAuth(user.Id.ToString(), inputModel.RememberMe);
                vm.Success = true;
                vm.Token = user.AuthToken.Token;
                vm.Expires = user.AuthToken.Expires;
                vm.User = new UserViewModel(user);
            } else
            {
                vm.Errors.Add("AuthFail1");

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Security,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "Authentication failed using email: " + inputModel.Email
                });
            }

            return vm;
        }

        /// <summary>
        /// Attempt to sign a user into the system using a token
        /// </summary>
        /// <param name="inputModel">Sign in parameters</param>
        /// <returns>Sign in success/failure information</returns>
        [HttpPost]
        public AuthResponseModel Authenticate(AuthenticateInputModel inputModel)
        {
            var vm = new AuthResponseModel();
            var user = UserService.Authenticate(inputModel.Token);
            if (user != null)
            {
                if (user.Status != UserStatus.Active && user.Status != UserStatus.Suspended)
                {
                    vm.Errors.Add("Your account is pending approval.");
                    return vm;
                }
                _auth.DoAuth(user.Id.ToString(), false);
                vm.Success = true;
                vm.Token = user.AuthToken.Token;
                vm.Expires = user.AuthToken.Expires;
                vm.User = new UserViewModel(user);
            }
            else
            {
                vm.Errors.Add("Invalid authentication token.");

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Security,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "Authentication failed using token: " + inputModel.Token
                });
            }

            return vm;
        }

        /// <summary>
        /// Sign user out of the system
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public BaseResponseModel SignOut()
        {

            // if user is not an early beta user, destroy auth token
            if (CurrentUser != null)
            {
                string tokenValue = HttpContext.Current.Request.Headers["authToken"];
                var betaUsers = _configService.AppSettings("BetaUserIds");
                var ids = betaUsers.Split(',').Select(x => Int32.Parse(x)).ToArray();
                if (!String.IsNullOrEmpty(tokenValue) && !ids.Contains(CurrentUser.Id))
                {
                    UserService.DestroyAuthToken(CurrentUser);
                }
            }

            _auth.SignOut();

            // TODO - API is not stateless.
            // The below implementation prevents the API from being
            // stateless. A better implementation would be OAuth or some other
            // kerberos/token method, however for the time being...

            // clear authentication cookie
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            authCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(authCookie);
            
            // clear session cookie
            var sessionCookie = new HttpCookie("ASP.NET_SessionId", "");
            sessionCookie.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(sessionCookie);

            // clear token
            var authToken = new HttpCookie("authToken", "");
            authToken.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Current.Response.Cookies.Add(authToken);

            var vm = new BaseResponseModel
            {
                Success = true
            };
            return vm;
        }

        /// <summary>
        /// Update a user's information.
        /// </summary>
        /// <param name="inputModel">Info to update</param>
        /// <returns></returns>
        [HttpPost, RequiresApiAuth]
        public BaseResponseModel Update(UpdateUserInputModel inputModel)
        {
            var vm = new BaseResponseModel();

            // Validate request
            var validationState = new ValidationDictionary();
            inputModel.ValidateRequest(validationState);

            // Get existing user
            var user = UserService.GetUserById(inputModel.UserId);
            var merchant = UserService.GetUserByMerchantId(inputModel.MerchantId);
            if (user == null)
            {
                throw new HttpException(404, "User not found.");
            }

            // Do not allow editing of users other than yourself if you
            // don't have permissions
            if (!CurrentUser.HasPermission(Permission.EditUsers)
                && user.Id != CurrentUser.Id)
            {
                if(user.Merchant.Id != CurrentUser.Id)
                {
                    throw new HttpException(401, "You do not have permissions to complete this action.");    
                }
                
            }

            // Copy properties
            if (inputModel.MerchantId != null) user.MerchantId = inputModel.MerchantId;

            bool emailChanged = false;

            if (inputModel.Type == Models.UpdateUserInputModel.UpdateType.Profile)
            {
                if (inputModel.Email != null)
                {
                    emailChanged = user.Email != inputModel.Email;
                    user.Email = inputModel.Email;
                }

                if (inputModel.EnableGratuity.HasValue) user.Settings.EnableGratuity = inputModel.EnableGratuity.Value;

                if (inputModel.FirstName != null)
                {
                    user.FirstName = inputModel.FirstName;
                }
                if (inputModel.LastName != null)
                {
                    user.LastName = inputModel.LastName;
                }
                if (user.Company != null)
                {
                    user.Company.Name = inputModel.CompanyName;
                    user.Company.Street = inputModel.Street;
                    user.Company.Street2 = inputModel.Street2;
                    user.Company.State = inputModel.State;
                    user.Company.Zip = inputModel.Zip;
                    user.Company.Phone = inputModel.Phone;
                    user.Company.Industry = inputModel.Industry;
                    user.Company.Website = inputModel.Website;
                }
            } else if (inputModel.Type == Models.UpdateUserInputModel.UpdateType.Design)
            {
                user.Settings.BackgroundColor = inputModel.BackgroundColor;
                user.Settings.StoreTitle = inputModel.StoreTitle;
                user.Settings.LogoUrl = inputModel.LogoUrl;
                user.Settings.HtmlTemplate = inputModel.HtmlTemplate;
                if (inputModel.HtmlTemplate == "{{empty}}") user.Settings.HtmlTemplate = null;
            }

            bool changeStatus = false;
            bool updateSubscription = false;

            // Additional properties for admin users
            if (CurrentUser.HasPermission(Permission.EditUsers))
            {
                if (inputModel.Role.HasValue) user.Role = inputModel.Role.Value;
                if (inputModel.Status.HasValue && inputModel.Status.Value != user.Status)
                {
                    changeStatus = true;
                }

                // Admin has changed or created subscription
                if (inputModel.SubscriptionType.HasValue
                    && (user.Subscription == null || inputModel.SubscriptionType != user.Subscription.Type))
                {
                    var expirationDate = inputModel.SubscriptionDateExpires.HasValue 
                                            ? inputModel.SubscriptionDateExpires.Value : DateTime.UtcNow.AddMonths(1);
                    decimal price = Decimal.Parse(_configService.AppSettings("Subscription." + inputModel.SubscriptionType.Value + ".Price"));
                    if (user.Subscription == null)
                    {
                        user.Subscription = new Subscription
                       {
                           Price = price,
                           Type = inputModel.SubscriptionType.Value,
                           Term = SubscriptionTerm.Monthly,
                           Status = SubscriptionStatus.Active,
                           DateSubscribed = DateTime.UtcNow,
                           DateExpires = expirationDate,
                           DateRenewed = DateTime.UtcNow
                       };
                        if (inputModel.SubscriptionType.Value == SubscriptionType.Unlimited) user.Subscription.DateExpires = DateTime.UtcNow.AddYears(50);
                    }
                    else
                    {
                        user.Subscription.Type = inputModel.SubscriptionType.Value;
                        user.Subscription.DateRenewed = DateTime.UtcNow;
                        user.Subscription.DateExpires = expirationDate;
                    }
                    updateSubscription = true;
                }
                else if (!inputModel.SubscriptionType.HasValue)
                {
                    user.Subscription = null;
                }
            }

            if (UserService.ValidateUser(user, validationState))
            {
                if (changeStatus) UserService.UpdateStatus(user, inputModel.Status.Value);

                if (updateSubscription) _subscriptionService.Save(user.Subscription);

                string newPass = String.IsNullOrWhiteSpace(inputModel.Password) ? null : inputModel.Password;
                UserService.UpdateUser(user, newPass);
                if (CurrentUser.Id == user.Id && emailChanged)
                {
                    ReAuthorizeUser(inputModel.Email);
                }

                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "User " + inputModel.Email + " (ID #" + user.Id + ") was updated.",
                    User = CurrentUser
                });

                vm.Success = true;
            }

            vm.Errors = validationState.Errors;
            return vm;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Log a user back into the system after their email has changed.
        /// </summary>
        /// <param name="email"></param>
        private void ReAuthorizeUser(string email)
        {
            bool rememberMe = false;

            // Retrieve "remember me" cookie
            var cookieName = FormsAuthentication.FormsCookieName;
            var request = HttpContext.Current.Request;
            var cookie = request.Cookies.Get(cookieName);
            if (cookie != null)
            {
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                rememberMe = ticket.IsPersistent;
            }
            var user = UserService.GetUserByEmail(email);
            _auth.DoAuth(user.Id.ToString(), rememberMe);
        }

        #endregion
    }

}
