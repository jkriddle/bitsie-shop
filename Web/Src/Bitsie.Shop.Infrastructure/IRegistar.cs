using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace Bitsie.Shop.Infrastructure
{
    public interface IRegistar
    {
        void Register(WindsorContainer windsorContainer);
    }

}
