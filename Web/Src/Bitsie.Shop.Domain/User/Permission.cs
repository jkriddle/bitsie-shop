using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public class Permission : Entity
    {
        public static string EditOrders = "edit-orders";
        public static string EditUsers = "edit-users";
        public static string EditCustomers = "edit-customers";
        public static string EditProducts = "edit-products";

        /// <summary>
        /// Unique permission name that matches static permissions
        /// defined in this class.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// User-friendly description of this permission.
        /// </summary>
        public virtual string Description { get; set; }
    }
}