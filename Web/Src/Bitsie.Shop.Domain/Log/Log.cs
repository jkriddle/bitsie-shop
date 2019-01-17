using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public class Log : Entity
    {
        public virtual DateTime LogDate { get; set; }
        public virtual LogCategory Category { get; set; }
        public virtual LogLevel Level { get; set; }
        public virtual string Message { get; set; }
        public virtual string Details { get; set; }
        public virtual string IpAddress { get; set; }
        public virtual User User { get; set; }

        public Log()
        {
            LogDate = DateTime.UtcNow;
        }
    }
}
