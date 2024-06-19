using Xunit;
using ETL.Transform;
using ETL.Transform.TransformedModels;
using CommonData.Models;
using System.Collections.Generic;

namespace ETL.UnitTests
{
    public class OrderDetailsTransformationTests
    {
        private readonly OrderTransform _orderTransform;

        public OrderDetailsTransformationTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Fact]
        public void FilterOrdersByDateRange_ShouldFilterCorrectly()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { OrderDate = new DateTime(2021, 1, 1) },
        new Order { OrderDate = new DateTime(2021, 2, 1) },
        new Order { OrderDate = new DateTime(2021, 3, 1) }
    };

            var startDate = new DateTime(2021, 1, 15);
            var endDate = new DateTime(2021, 3, 15);

            // Act
            var filteredOrders = _orderTransform.FilterOrdersByDateRange(orders, startDate, endDate).ToList();

            // Assert
            Assert.Equal(2, filteredOrders.Count); 
            Assert.Equal(new DateTime(2021, 2, 1), filteredOrders[0].OrderDate);
            Assert.Equal(new DateTime(2021, 3, 1), filteredOrders[1].OrderDate);
            Assert.Equal(new DateTime(2021, 2, 1), filteredOrders.First().OrderDate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SortOrdersByDate_ShouldSortCorrectly(bool ascending)
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { OrderDate = new DateTime(2021, 2, 1) },
        new Order { OrderDate = new DateTime(2021, 1, 1) },
        new Order { OrderDate = new DateTime(2021, 3, 1) }
    };

            // Act
            var sortedOrders = _orderTransform.SortOrdersByDate(orders, ascending).ToList();

            // Assert
            if (ascending)
            {
                Assert.Equal(new DateTime(2021, 1, 1), sortedOrders.First().OrderDate);
                Assert.Equal(new DateTime(2021, 3, 1), sortedOrders.Last().OrderDate);
            }
            else
            {
                Assert.Equal(new DateTime(2021, 3, 1), sortedOrders.First().OrderDate);
                Assert.Equal(new DateTime(2021, 1, 1), sortedOrders.Last().OrderDate);
            }
        }




    }
}

