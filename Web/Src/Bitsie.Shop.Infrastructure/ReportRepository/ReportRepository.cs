using System;
using System.Collections.Generic;
using Bitsie.Shop.Domain.Filters;
using NHibernate.Criterion;
using Bitsie.Shop.Domain;
using NHibernate.Transform;
using System.Linq;

namespace Bitsie.Shop.Infrastructure
{
    public class ReportRepository : IReportRepository
    {
        private static decimal MINIMUM_PAYOUT_THRESHOLD = 20.00m;

        public DashboardReport GetDashboardReport(int userId)
        {
            DashboardReport report;
            var session = SessionFactory.Instance.GetCurrentSession();
            var query = session.GetNamedQuery("GetDashboardReport");
            query.SetParameter("UserId", userId);
            query.SetResultTransformer(Transformers.AliasToBean(typeof(DashboardReport)));
            report = query.UniqueResult<DashboardReport>();
            return report;
        }

        public IList<PayoutReport> GetPayoutReport(PayoutReportFilter filter)
        {
            IList<PayoutReport> results = null;
            var session = SessionFactory.Instance.GetCurrentSession();
            var query = session.GetNamedQuery("GetPayoutBalances");
            query.SetParameter("StartDate", filter.StartDate);
            query.SetParameter("EndDate", filter.EndDate);
            query.SetParameter("PaymentMethod", filter.PaymentMethod);
            query.SetResultTransformer(Transformers.AliasToBean(typeof(PayoutReport)));
            results = query.List<PayoutReport>();
            if (filter.AboveMinimumThreshold)
            {
                results = results.Where(r => r.PayoutBalance >= MINIMUM_PAYOUT_THRESHOLD).ToList();
            }
            return results;
        }
    }
}
