using System.ComponentModel;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public enum Role
    {
        [Description("Merchant")]
        Merchant = 1,
        [Description("Administrator")]
        Administrator,
        [Description("Tipsie")]
        Tipsie,
        [Description("Customer")]
        Customer
    }
}