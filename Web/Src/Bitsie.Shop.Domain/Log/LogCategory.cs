using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public enum LogCategory
    {
        /// <summary>
        /// Hardware or IIS level logs.
        /// </summary>
        [Description("System")]
        System = 1,

        /// <summary>
        /// Application logs (most common).
        /// </summary>
        [Description("Application")]
        Application,

        /// <summary>
        /// Security adits
        /// </summary>
        [Description("Security")]
        Security,

        /// <summary>
        /// Emails being sent
        /// </summary>
        [Description("Email")]
        Email
    }
}
