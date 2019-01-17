using Bitsie.Shop.Domain;
using Bitsie.Shop.Domain.Filters;
using Bitsie.Shop.Infrastructure;
using System.Collections.Generic;

namespace Bitsie.Shop.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public DashboardReport GetDashboardReport(int userId)
        {
            return _reportRepository.GetDashboardReport(userId);
        }

        public IList<PayoutReport> GetPayoutReport(PayoutReportFilter filter)
        {
            return _reportRepository.GetPayoutReport(filter);
        }
    }
}
