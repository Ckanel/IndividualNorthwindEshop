using Xunit;
using ETL.Transform;
using ETL.Transform.TransformedModels;
using CommonData.Models;
using System.Collections.Generic;
using System.Linq;

namespace ETL.UnitTests
{
    public class GroupingAndAnalyticsTests
    {
        private readonly OrderTransform _orderTransform;

        public GroupingAndAnalyticsTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Fact]
        public void GroupOrdersByCustomer_ShouldGroupCorrectly()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { CustomerId = "C1" },
                new Order { CustomerId = "C2" },
                new Order { CustomerId = "C1" }
            };

            // Act
            var groupedOrders = _orderTransform.GroupOrdersByCustomer(orders);

            // Assert
            Assert.Equal(2, groupedOrders.Count());
            Assert.Equal(2, groupedOrders.First(g => g.Key == "C1").Count());
        }
        [Fact]
        public void GroupOrdersByCustomers_ShouldGroupCorrectly()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { CustomerId = "C1", OrderId = 1, OrderDate = new DateTime(2021, 1, 1) },
        new Order { CustomerId = "C1", OrderId = 2, OrderDate = new DateTime(2021, 2, 1) },
        new Order { CustomerId = "C2", OrderId = 3, OrderDate = new DateTime(2021, 3, 1) }
    };

            // Act
            var groupedOrders = _orderTransform.GroupOrdersByCustomer(orders);

            // Assert
            Assert.Collection(groupedOrders,
                g =>
                {
                    Assert.Equal("C1", g.Key);
                    Assert.Equal(2, g.Count());
                },
                g =>
                {
                    Assert.Equal("C2", g.Key);
                    Assert.Single(g);
                });
        }



        [Fact]
        public void CalculateEmployeePerformance_ShouldCalculateCorrectly()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { EmployeeId = 1, OrderDate = new DateTime(2021, 1, 1), ShippedDate = new DateTime(2021, 1, 10) },
        new Order { EmployeeId = 1, OrderDate = new DateTime(2021, 2, 1), ShippedDate = new DateTime(2021, 2, 5) },
        new Order { EmployeeId = 2, OrderDate = new DateTime(2021, 3, 1), ShippedDate = new DateTime(2021, 3, 3) }
    };

            var employees = new List<Employee>
    {
        new Employee { EmployeeId = 1, FirstName = "John", LastName = "Doe" },
        new Employee { EmployeeId = 2, FirstName = "Jane", LastName = "Smith" }
    };

            // Act
            var performance = _orderTransform.CalculateEmployeePerformance(orders, employees);

            // Assert
            var employee1 = performance.First(e => e.EmployeeId == 1);
            var employee2 = performance.First(e => e.EmployeeId == 2);

            Assert.Equal(2, employee1.OrdersHandled);
            Assert.Equal(6.5, employee1.AverageHandlingTime);
            Assert.Equal("John", employee1.EmployeeFirstName);
            Assert.Equal("Doe", employee1.EmployeeLastName);

            Assert.Equal(1, employee2.OrdersHandled);
            Assert.Equal(2, employee2.AverageHandlingTime);
            Assert.Equal("Jane", employee2.EmployeeFirstName);
            Assert.Equal("Smith", employee2.EmployeeLastName);
        }
    }
}

