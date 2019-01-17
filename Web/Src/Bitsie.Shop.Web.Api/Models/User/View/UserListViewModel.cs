using System.Collections.Generic;
using Bitsie.Shop.Services;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class UserListViewModel : PagedViewModel<User>
    {
        #region Constructor

        public UserListViewModel(IPagedList<User> users)
            : base(users)
        {
            Users = new List<UserViewModel>();
            foreach (User user in users.Items)
            {
                Users.Add(new UserViewModel(user));
            }
        }

        #endregion

        #region Public Properties

        public IList<UserViewModel> Users { get; set; }

        #endregion
    }
}