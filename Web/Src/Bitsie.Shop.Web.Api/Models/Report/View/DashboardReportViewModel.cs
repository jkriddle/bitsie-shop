using Bitsie.Shop.Domain;

namespace Bitsie.Shop.Web.Api.Models
{
    public class DashboardReportViewModel
    {
        private readonly User _user;
        private readonly DashboardReport _report;

        public DashboardReportViewModel(User user, DashboardReport report)
        {
            _user = user;
            _report = report;
        }

        public PaymentMethod? PaymentMethod { get { return _user.Settings.PaymentMethod; } }
        public decimal RevenueTodayUsd { get { return _report.RevenueTodayUsd; } }
        public decimal RevenueTodayBtc { get { return _report.RevenueTodayBtc; } }
        public decimal RevenueThisMonthUsd { get { return _report.RevenueThisMonthUsd; } }
        public decimal RevenueThisMonthBtc { get { return _report.RevenueThisMonthBtc; } }
        public decimal RevenueAllTimeUsd { get { return _report.RevenueAllTimeUsd; } }
        public decimal RevenueAllTimeBtc { get { return _report.RevenueAllTimeBtc; } }
        public decimal RevenuePendingPayoutUsd { get { return _report.RevenuePendingPayoutUsd; } }
        public decimal RevenuePendingPayoutBtc { get { return _report.RevenuePendingPayoutBtc; } }
    }
}