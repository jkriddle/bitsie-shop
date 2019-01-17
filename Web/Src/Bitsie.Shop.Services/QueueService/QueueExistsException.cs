using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public class QueueExistsException : Exception
    {
        public QueueExistsException(string message) : base(message) { }
    }
}
