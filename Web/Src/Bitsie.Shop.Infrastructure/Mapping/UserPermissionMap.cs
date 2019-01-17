using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Infrastructure.Mapping
{
    public class UserPermissionMap : IAutoMappingOverride<UserPermission>
    {
        public void Override(AutoMapping<UserPermission> mapping)
        {
            mapping.Table("UserPermissions");
            mapping.Id(x => x.Id, "UserPermissionID");
            mapping.References(x => x.User).Column("UserID").Not.Nullable();
            mapping.References(x => x.Permission).Column("PermissionID").Not.Nullable();
        }
    }
}
