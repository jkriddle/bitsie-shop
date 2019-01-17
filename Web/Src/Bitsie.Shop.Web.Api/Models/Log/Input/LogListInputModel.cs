using System;
using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class LogListInputModel : PagedInputModel
    {
        /// <summary>
        /// Filter logs created from a specific user's actions
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Filter a specific category
        /// </summary>
        public LogCategory? LogCategory { get; set; }

        /// <summary>
        /// Filter a specific level
        /// </summary>
        public LogLevel? LogLevel { get; set; }

        /// <summary>
        /// Filter for a date (earliest)
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter for a date (latest)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Search details field
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Search message field
        /// </summary>
        public string Message { get; set; }
    }
}