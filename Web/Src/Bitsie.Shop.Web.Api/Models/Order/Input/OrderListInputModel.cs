using System;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class OrderListInputModel : PagedInputModel
    {

        /// <summary>
        /// Filter orders after a specific ID
        /// </summary>
        public int? AfterId { get; set; }

        /// <summary>
        /// Filter orders by a specific ID
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// Filter orders by a specific user
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Filter orders by a specific status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Filter orders after a specific start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter orders before a specific end date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Specific order report we are retrieving (e.g. gratuity report)
        /// </summary>
        public string Report { get; set; }

        public string View { get; set; }
    }
}