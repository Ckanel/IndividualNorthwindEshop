using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETL.Transform.TransformedModels
{
    public class TransformedOrderDetail
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal TotalAmount { get; set; }
        // Product Name

    }
}
