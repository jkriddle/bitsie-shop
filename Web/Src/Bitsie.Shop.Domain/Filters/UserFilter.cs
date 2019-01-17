using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class UserFilter : BaseFilter
    {
        public UserFilter()
        {
            SortColumn = "Email";
        }

        public string Email { get; set; }
        public User Merchant { get; set; }
        public Role? Role { get; set; }
    }
}
