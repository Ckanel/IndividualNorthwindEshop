using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{
    public class SalesForecast
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal PredictedSales { get; set; }
    }
}
