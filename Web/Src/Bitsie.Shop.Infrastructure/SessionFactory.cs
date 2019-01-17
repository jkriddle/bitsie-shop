using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Infrastructure
{
    public static class SessionFactory
    {
        public static ISessionFactory Instance { get; set; }
    }
}
