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

namespace Bitsie.Shop.Web.Api.Controllers
{
    [NoCache]
    public class OfflineAddressController : BaseApiController
    {
        #region Fields

        private readonly IAuth _auth;
        private readonly IMapperService _mapper;

        #endregion

        #region Constructor

        public OfflineAddressController(IAuth auth, 
            IUserService userService, 
            ILogService logService,
            IMapperService mapper) : base(userService, logService)
        {
            _auth = auth;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieve a list of users based on filter criteria
        /// </summary>
        /// <param name="inputModel">Filter criteria</param>
        /// <returns>Paged list of users</returns>
        [HttpGet, RequiresApiAuth]
        public OfflineAddressListViewModel Get()
        {
            return new OfflineAddressListViewModel(CurrentUser.OfflineAddresses.Where(o => o.Status == OfflineAddressStatus.Active).ToList());
        }

        /// <summary>
        /// Get Employees Tipse Card Address
        /// </summary>
        /// <param name="inputModel">User ID</param>
        /// <returns></returns>
        [HttpPost, RequiresApiAuth, ApiRequiresRole(Role.Administrator, Role.Merchant)]
        public OfflineAddressListViewModel GetTipsieAddress(int id)
        {
            User user = UserService.GetUserById(id);
            return new OfflineAddressListViewModel(user.OfflineAddresses.Where(o => o.Status == OfflineAddressStatus.Active).ToList());
        }

        [HttpPost, RequiresApiAuth]
        public UpdateOfflineAddressResponseModel Update(OfflineAddressInputModel inputModel)
        {
            // Validate request
            var validationDictionary = new ValidationDictionary();

            int numAddresses = inputModel.OfflineAddress == null ? 0 : inputModel.OfflineAddress.Count;
            if (numAddresses > 10)
            {
                validationDictionary.AddError("NumAddresses", "You cannot have more than 10 offline addresses.");
            }
            else
            {
                for (var i = 0; i < numAddresses; i++)
                {
                    bool create = false;
                    string address = inputModel.OfflineAddress.Count > i ? inputModel.OfflineAddress[i] : "";
                    OfflineAddress existing = CurrentUser.OfflineAddresses.FirstOrDefault(a => a.Address == address);
                    if (existing == null)
                    {
                        existing = new OfflineAddress();
                        create = true;
                    }
                    existing.EmailNotifications = inputModel.OfflineEmail[i];
                    string phone = String.IsNullOrEmpty(inputModel.OfflinePhone[i]) ? "" : inputModel.OfflinePhone[i];
                    existing.TextNotifications = Regex.Replace(phone, "[^0-9,]", "");
                    existing.Status = OfflineAddressStatus.Active;
                    if (create)
                    {
                        CurrentUser.OfflineAddresses.Add(existing);
                        UserService.SaveOfflineAddress(CurrentUser, existing);
                    }
                }
            }

            var vm = new UpdateOfflineAddressResponseModel(UserService.GetOfflineAddressesByUserId(CurrentUser.Id));

            if (validationDictionary.Errors.Count == 0 && UserService.ValidateUser(CurrentUser, validationDictionary))
            {
                UserService.UpdateUser(CurrentUser);
                LogService.CreateLog(new Log
                {
                    Category = LogCategory.Application,
                    IpAddress = GetClientIp(ControllerContext.Request),
                    Level = LogLevel.Info,
                    Message = "User " + CurrentUser.Email + " (ID #" + CurrentUser.Id + ") updated their offline addresses.",
                    User = CurrentUser
                });

                vm.Success = true;
            }

            vm.Errors = validationDictionary.Errors;
            return vm;
        }

        /// <summary>
        /// Mark user as deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, RequiresApiAuth, ApiRequiresRole(Role.Administrator, Role.Merchant)]
        public BaseResponseModel Delete(int id)
        {
            OfflineAddress existing = CurrentUser.OfflineAddresses.FirstOrDefault(a => a.Id == id);
            if (existing == null) throw new HttpException(404, "Address not found.");

            UserService.DeleteOfflineAddress(existing);

            var vm = new UpdateOfflineAddressResponseModel(UserService.GetOfflineAddressesByUserId(CurrentUser.Id));
            vm.Success = true;
            return vm;
        }

        #endregion

    }

}
