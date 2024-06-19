using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{
    public class ProductPerformance
    {
        public int ProductId { get; set; }
        public int TotalQuantitySold { get; set; }

        public decimal TotalAmountSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ProductName { get; set; }
    }
}
