using Bitsie.Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Build
{
    public class FakeNotificationService : INotificationService
    {
        public async Task<bool> Notify(string toAddress, string subject, string html, object messageParams)
        {
            // Do nothing
            await Task.Yield();
            return true;
        }
    }
}
