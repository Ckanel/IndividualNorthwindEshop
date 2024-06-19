using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using CommonData.Data;
using CommonData.Models;
using ETL.Extract;

namespace ETL.UnitTests
{
    public class OrderRepositoryTests
    {
        private readonly Mock<MasterContext> _mockContext;
        private readonly OrderRepository _orderRepository;

        public OrderRepositoryTests()
        {
            _mockContext = new Mock<MasterContext>();
            _orderRepository = new OrderRepository(_mockContext.Object);
        }

        private static DbSet<T> GetMockDbSet<T>(List<T> sourceList) where T : class
        {
            return sourceList.AsQueryable().AsQueryableMockDbSet().Object;
        }

        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderId = 1, OrderDate = new DateTime(2021, 1, 1) },
                new Order { OrderId = 2, OrderDate = new DateTime(2021, 2, 1) }
            };
            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetAllOrdersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.OrderId == 1);
            Assert.Contains(result, o => o.OrderId == 2);
        }

        [Fact]
        public async Task GetAllOrdersWithDetailsAsync_ShouldReturnAllOrdersWithDetails()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    OrderDate = new DateTime(2021, 1, 1),
                    Customer = new Customer { CustomerId = "C1" },
                    Employee = new Employee { EmployeeId = 1 },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            OrderId = 1,
                            Product = new Product { ProductId = 1 }
                        }
                    }
                },
                new Order
                {
                    OrderId = 2,
                    OrderDate = new DateTime(2021, 2, 1),
                    Customer = new Customer { CustomerId = "C2" },
                    Employee = new Employee { EmployeeId = 2 },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            OrderId = 2,
                            Product = new Product { ProductId = 2 }
                        }
                    }
                }
            };
            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetAllOrdersWithDetailsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.OrderId == 1 && o.Customer.CustomerId == "C1" && o.Employee.EmployeeId == 1);
            Assert.Contains(result, o => o.OrderId == 2 && o.Customer.CustomerId == "C2" && o.Employee.EmployeeId == 2);
        }

        [Fact]
        public async Task GetPagedOrdersAsync_ShouldReturnPagedOrders()
        {
            // Arrange
            var orders = new List<Order>
       {
           new Order { OrderId = 1, OrderDate = new DateTime(2021, 1, 1) },
           new Order { OrderId = 2, OrderDate = new DateTime(2021, 2, 1) },
           new Order { OrderId = 3, OrderDate = new DateTime(2021, 3, 1) }
       };
            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetPagedOrdersAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.OrderId == 1);
            Assert.Contains(result, o => o.OrderId == 2);

            // Verify interaction
            _mockContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task GetOrdersByDateRangeAsync_ShouldReturnOrdersWithinDateRange()
        {
            // Arrange
            var orders = new List<Order>
       {
           new Order { OrderId = 1, OrderDate = new DateTime(2021, 1, 1) },
           new Order { OrderId = 2, OrderDate = new DateTime(2021, 2, 1) },
           new Order { OrderId = 3, OrderDate = new DateTime(2021, 3, 1) }
       };
            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetOrdersByDateRangeAsync(new DateTime(2021, 1, 1), new DateTime(2021, 2, 28));

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.OrderId == 1);
            Assert.Contains(result, o => o.OrderId == 2);

            // Verify interaction
            _mockContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ShouldReturnOrdersForCustomer()
        {
            // Arrange
            var orders = new List<Order>
       {
           new Order { OrderId = 1, CustomerId = "C1", OrderDate = new DateTime(2021, 1, 1) },
           new Order { OrderId = 2, CustomerId = "C2", OrderDate = new DateTime(2021, 2, 1) }
       };
            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetOrdersByCustomerIdAsync("C1");

            // Assert
            Assert.Single(result);
            Assert.Contains(result, o => o.OrderId == 1 && o.CustomerId == "C1");

            // Verify interaction
            _mockContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public async Task GetOrdersByProductCategoryAsync_ShouldReturnOrders()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order
        {
            OrderId = 1,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Product = new Product
                    {
                        ProductId = 1,
                        CategoryId = 1, // Ensure CategoryId is set directly
                        Category = new Category { CategoryId = 1 }
                    }
                }
            }
        },
        new Order
        {
            OrderId = 2,
            OrderDetails = new List<OrderDetail>
            {
                new OrderDetail
                {
                    Product = new Product
                    {
                        ProductId = 2,
                        CategoryId = 2, // Ensure CategoryId is set directly
                        Category = new Category { CategoryId = 2 }
                    }
                }
            }
        }
    };

            var mockDbSet = GetMockDbSet(orders);

            // Set up the include properties
            _mockContext.Setup(c => c.Orders)
                .Returns(mockDbSet);

            // Act
            var result = await _orderRepository.GetOrdersByProductCategoryAsync(1);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, o => o.OrderId == 1);

            // Verify interaction
            _mockContext.Verify(c => c.Orders, Times.Once);
        }



        // Unit test for GetCustomersWithOrdersAsync
        [Fact]
        public async Task GetCustomersWithOrdersAsync_ShouldReturnCustomersWithOrders()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { CustomerId = "C1", Orders = new List<Order> { new Order { OrderId = 1 } } },
                new Customer { CustomerId = "C2", Orders = new List<Order> { new Order { OrderId = 2 } } }
            };

            _mockContext.Setup(c => c.Customers).Returns(GetMockDbSet(customers));

            // Act
            var result = await _orderRepository.GetCustomersWithOrdersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.CustomerId == "C1" && c.Orders.Any(o => o.OrderId == 1));
            Assert.Contains(result, c => c.CustomerId == "C2" && c.Orders.Any(o => o.OrderId == 2));

            // Verify interaction
            _mockContext.Verify(c => c.Customers, Times.Once);
        }

        // Unit test for GetProductsWithDetailsAsync
        [Fact]
        public async Task GetProductsWithDetailsAsync_ShouldReturnProductsWithDetails()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = 1, Category = new Category { CategoryId = 1 }, Supplier = new Supplier { SupplierId = 1 } },
                new Product { ProductId = 2, Category = new Category { CategoryId = 2 }, Supplier = new Supplier { SupplierId = 2 } }
            };

            _mockContext.Setup(c => c.Products).Returns(GetMockDbSet(products));

            // Act
            var result = await _orderRepository.GetProductsWithDetailsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductId == 1 && p.Category.CategoryId == 1 && p.Supplier.SupplierId == 1);
            Assert.Contains(result, p => p.ProductId == 2 && p.Category.CategoryId == 2 && p.Supplier.SupplierId == 2);

            // Verify interaction
            _mockContext.Verify(c => c.Products, Times.Once);
        }

        // Unit test for GetOrderDetailsByDateRangeAsync
        [Fact]
        public async Task GetOrderDetailsByDateRangeAsync_ShouldReturnOrderDetailsWithinDateRange()
        {
            // Arrange
            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { OrderId = 1, Order = new Order { OrderDate = new DateTime(2021, 1, 1) } },
                new OrderDetail { OrderId = 2, Order = new Order { OrderDate = new DateTime(2021, 2, 1) } },
                new OrderDetail { OrderId = 3, Order = new Order { OrderDate = new DateTime(2021, 3, 1) } }
            };

            _mockContext.Setup(c => c.OrderDetails).Returns(GetMockDbSet(orderDetails));

            // Act
            var result = await _orderRepository.GetOrderDetailsByDateRangeAsync(new DateTime(2021, 1, 1), new DateTime(2021, 2, 28));

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, od => od.OrderId == 1);
            Assert.Contains(result, od => od.OrderId == 2);

            // Verify interaction
            _mockContext.Verify(c => c.OrderDetails, Times.Once);
        }

        // Unit test for GetPagedOrdersWithDetailsAsync
        [Fact]
        public async Task GetPagedOrdersWithDetailsAsync_ShouldReturnPagedOrdersWithDetails()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    Customer = new Customer { CustomerId = "C1" },
                    Employee = new Employee { EmployeeId = 1 },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Product = new Product { ProductId = 1 } }
                    }
                },
                new Order
                {
                    OrderId = 2,
                    Customer = new Customer { CustomerId = "C2" },
                    Employee = new Employee { EmployeeId = 2 },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Product = new Product { ProductId = 2 } }
                    }
                },
                new Order
                {
                    OrderId = 3,
                    Customer = new Customer { CustomerId = "C3" },
                    Employee = new Employee { EmployeeId = 3 },
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { Product = new Product { ProductId = 3 } }
                    }
                }
            };

            _mockContext.Setup(c => c.Orders).Returns(GetMockDbSet(orders));

            // Act
            var result = await _orderRepository.GetPagedOrdersWithDetailsAsync(1, 2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, o => o.OrderId == 1);
            Assert.Contains(result, o => o.OrderId == 2);

            // Verify interaction
            _mockContext.Verify(c => c.Orders, Times.Once);
        }
    }
}

