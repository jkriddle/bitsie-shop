using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Domain.Filters
{
    public class LogFilter : BaseFilter
    {
        public LogFilter()
        {
            SortColumn = "LogDate";
            SortDirection = Domain.SortDirection.Descending;
        }

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
        /// Filter logs from this date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter logs up to this date
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
