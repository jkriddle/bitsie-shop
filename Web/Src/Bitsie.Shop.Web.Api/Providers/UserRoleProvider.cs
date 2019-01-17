using System;
using System.Configuration.Provider;
using System.Web.Security;
using Bitsie.Shop.Domain;
using Bitsie.Shop.Services;
using Microsoft.Practices.ServiceLocation;

namespace Bitsie.Shop.Web.Api.Providers
{
    public class UserRoleProvider : RoleProvider
    {
        private IUserService _userService;

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _userService = ServiceLocator.Current.GetInstance<IUserService>();

            if (_userService == null)
                throw new ProviderException("RoleProvider only works when used in combination with an IUserService. You should add this component to your container.");

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = typeof(UserMembershipProvider).Name;

            base.Initialize(name, config);

        }

        #region "Not Implemented"

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string[] GetRolesForUser(string id)
        {
                User user = _userService.GetUserById(Int32.Parse(id));
            string[] roles = new string[0];
            if (user != null)
            {
                roles = new string[1];
                roles[0] = ((Role)user.Role).ToString();
            }
            return roles;
        }

        public override bool IsUserInRole(string id, string roleName)
        {
            User currentUser = _userService.GetUserById(Int32.Parse(id));
            if (currentUser != null)
            {
                return currentUser.Role.ToString() == roleName;
            }
            return false;
        }
    }
}