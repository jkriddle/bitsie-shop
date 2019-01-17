using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using System;
using System.Collections.Generic;
namespace Bitsie.Shop.Infrastructure
{
    public interface IReportRepository
    {
        DashboardReport GetDashboardReport(int userId);
        IList<PayoutReport> GetPayoutReport(PayoutReportFilter filter);
    }
}
