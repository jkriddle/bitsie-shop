using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Bitsie.Shop.Infrastructure
{
    public class SqlServerDialectWithFts : NHibernate.Spatial.Dialect.MsSql2008GeographyDialect
    {
        public SqlServerDialectWithFts()
            : base()
        {
            RegisterFunction("freetext", new StandardSQLFunction("freetext", null));
            RegisterFunction("contains", new StandardSQLFunction("contains", null));
        }
    }
}
