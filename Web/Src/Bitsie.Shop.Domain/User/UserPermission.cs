using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public class UserPermission : Entity
    {
        public virtual User User { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
