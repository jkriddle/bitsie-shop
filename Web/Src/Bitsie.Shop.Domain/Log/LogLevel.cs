using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public enum LogLevel
    {
        [Description("Debug")]
        Debug = 1,
        [Description("Info")]
        Info,
        [Description("Warning")]
        Warning,
        [Description("Error")]
        Error
    }
}
