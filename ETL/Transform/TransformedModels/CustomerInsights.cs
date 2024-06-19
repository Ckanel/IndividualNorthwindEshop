using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{
    public class CustomerInsights
    {
        public string CustomerId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }


}
