using ETL.Transform.TransformedModels;

namespace IndividualNorthwindEshop.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<SalesData> SalesData { get; set; }
        public IEnumerable<ProductPerformance> ProductPerformance { get; set; }
        public IEnumerable<CustomerInsights> CustomerInsights { get; set; }
        public IEnumerable<DiscountEffectiveness> DiscountEffectiveness { get; set; }
        public IEnumerable<SalesForecast> SalesForecast { get; set; }
        public IEnumerable<EmployeePerformance> EmployeePerformance { get; set; }

    }
}
