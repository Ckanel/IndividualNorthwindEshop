using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{

    public class EmployeePerformance
    {
        public int EmployeeId { get; set; }
        public int OrdersHandled { get; set; }
        public double AverageHandlingTime { get; set; }
        public string EmployeeFirstName { get; set; } 
        public string EmployeeLastName { get; set; }
    }
}
