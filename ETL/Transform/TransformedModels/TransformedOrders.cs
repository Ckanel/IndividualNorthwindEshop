using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{
    public class TransformedOrder
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string ShipName { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public decimal TotalOrderAmount { get; set; }
        public int? EmployeeId { get; set; }
    }
}

