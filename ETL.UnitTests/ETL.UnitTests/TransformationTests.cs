using Xunit;
using ETL.Transform;
using ETL.Transform.TransformedModels;
using CommonData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ETL.UnitTests
{
    public class TransformationTests
    {
        private readonly OrderTransform _orderTransform;

        public TransformationTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Fact]
        public void TransformOrders_ShouldTransformOrdersCorrectly()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    OrderDate = DateTime.Now,
                    ShipName = null,
                    ShipAddress = null,
                    ShipCity = null,
                    ShipRegion = null,
                    CustomerId = null,
                    ShipVia = null,
                    ShipPostalCode = null,
                    ShipCountry = null,
                    Customer = new Customer { CompanyName = "Test Company", ContactName = "John Doe" },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { UnitPrice = 10M, Quantity = 2, Discount = 0.1F }
                    }
                }
            };

            Debug.WriteLine("Starting test...");

            // Act
            var transformedOrders = _orderTransform.TransformOrders(orders).ToList();

            Debug.WriteLine("Finished transformation...");

            // Assert
            Assert.Single(transformedOrders);
            var transformedOrder = transformedOrders.First();
            Assert.Equal("N/A", transformedOrder.ShipName);
            Assert.Equal("N/A", transformedOrder.ShipAddress);
            Assert.Equal("N/A", transformedOrder.ShipCity);
            Assert.Equal("N/A", transformedOrder.ShipRegion);
            Assert.Equal("Guest", transformedOrder.CustomerId);
            Assert.Equal("N/A", transformedOrder.ShipPostalCode);
            Assert.Equal("N/A", transformedOrder.ShipCountry);
            Assert.Equal("Test Company", transformedOrder.CustomerName);
            Assert.Equal("John Doe", transformedOrder.CustomerContact);
            Assert.Equal(18, transformedOrder.TotalOrderAmount); // (10 * 2) * 0.9 = 18
        }
    }
}

