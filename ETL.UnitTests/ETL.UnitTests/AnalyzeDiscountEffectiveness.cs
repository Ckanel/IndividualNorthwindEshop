using ETL.Transform;
using CommonData.Models;
namespace ETL.UnitTests
{
    public class AnalyzeDiscountEffectivenessTests
    {
        private readonly OrderTransform _orderTransform;

        public AnalyzeDiscountEffectivenessTests()
        {
            _orderTransform = new OrderTransform();
        }
        [Fact]
        public void PredictSales_ShouldProvideAccurateForecasts()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderDate = new DateTime(2021, 1, 15), 
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 100, Quantity = 1, Discount = 0 } } },
                new Order { OrderDate = new DateTime(2021, 2, 15),
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 200, Quantity = 1, Discount = 0 } } },
                new Order { OrderDate = new DateTime(2021, 3, 15),
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 300, Quantity = 1, Discount = 0 } } },
                new Order { OrderDate = new DateTime(2021, 4, 15),
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 400, Quantity = 1, Discount = 0 } } },
                new Order { OrderDate = new DateTime(2021, 5, 15),
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 500, Quantity = 1, Discount = 0 } } },
                new Order { OrderDate = new DateTime(2021, 6, 15),
                    OrderDetails = new List<OrderDetail> { new OrderDetail { UnitPrice = 600, Quantity = 1, Discount = 0 } } }
            };

            
            // First, transform orders to sales data
            var totalSales = _orderTransform.CalculateTotalSales(orders);

            // Then, predict sales using the transformed total sales data
            var forecast = _orderTransform.PredictSales(totalSales).ToList();

            // Assert
            // Verify forecast calculations match expected values
            Assert.Collection(forecast,
             f =>
             {
                 Assert.Equal(2021, f.Year);
                 Assert.Equal(4, f.Month);
                 Assert.Equal(200, f.PredictedSales); // Should be the average of the 3months before(1,2,3).
             },
             f =>
             {
                 Assert.Equal(2021, f.Year);
                 Assert.Equal(5, f.Month);
                 Assert.Equal(300, f.PredictedSales); // Should be the average of the 3 months before (2,3,4)
             },
             f =>
             {
                 Assert.Equal(2021, f.Year);
                 Assert.Equal(6, f.Month);
                 Assert.Equal(400, f.PredictedSales); // Should be the average of the 3 months before(3,4,5)
             });
        }
    }
}

