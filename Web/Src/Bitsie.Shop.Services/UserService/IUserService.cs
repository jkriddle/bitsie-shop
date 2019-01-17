using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using System.Collections.Generic;

namespace Bitsie.Shop.Services
{
    
    public interface IUserService
    {
        User Authenticate(string token);
        User Authenticate(string username, string password);
        void CreateUser(User user);
        void DeleteUser(User user);
        void GenerateUserPassword(User user, string plainTextPassword);
        void GenerateResetRequest(User user);
        User GetUserById(int id);
        User GetUserByMerchantId(string id);
        User GetUserByEmail(string email);
        User GetUserByEmail(string email, User merchant = null);
        User GetUserByResetToken(string token);
        IList<OfflineAddress> GetOfflineAddressesByUserId(int id);
        IPagedList<User> GetUsers(UserFilter filter, int currentPage, int numPerPage);
        void ResetPassword(User user, string newPassword);
        void UpdateUser(User user, string newPassword = null);
        void UpdateStatus(User user, UserStatus newStatus);
        void DeleteOfflineAddress(OfflineAddress address);
        bool ValidateUser(User user, IValidationDictionary validationDictionary);
        void SaveOfflineAddress(User user, OfflineAddress address);
        string CreateGuid(int size);
        void DestroyAuthToken(User user);
    }
}
