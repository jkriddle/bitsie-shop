using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain
{
    public enum UserStatus
    {
        Pending = 1,
        AwaitingApproval,
        Active,
        Suspended
    }
}
