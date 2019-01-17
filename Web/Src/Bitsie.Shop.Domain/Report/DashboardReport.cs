namespace Bitsie.Shop.Domain
{
    public class DashboardReport
    {
        public decimal RevenueTodayUsd { get; set; }
        public decimal RevenueTodayBtc { get; set; }
        public decimal RevenueThisMonthUsd { get; set; }
        public decimal RevenueThisMonthBtc { get; set; }
        public decimal RevenueAllTimeUsd { get; set; }
        public decimal RevenueAllTimeBtc { get; set; }
        public decimal RevenuePendingPayoutUsd { get; set; }
        public decimal RevenuePendingPayoutBtc { get; set; }
    }
}
