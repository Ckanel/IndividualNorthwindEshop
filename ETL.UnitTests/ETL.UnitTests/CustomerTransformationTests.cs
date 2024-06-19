using Xunit;
using ETL.Transform;
using ETL.Transform.TransformedModels;
using CommonData.Models;
using System.Collections.Generic;

namespace ETL.UnitTests
{
    public class CustomerTransformationTests
    {
        private readonly OrderTransform _orderTransform;

        public CustomerTransformationTests()
        {
            _orderTransform = new OrderTransform();
        }

        
        [Fact]
        public void TransformCustomers_ShouldTransformCustomersCorrectly()
        {
            // Arrange
            var customers = new List<Customer>
    {
        new Customer
        {
            CompanyName = null,
            ContactName = null,
            ContactTitle = null,
            Address = null,
            City = null,
            Region = null,
            PostalCode = null,
            Country = null,
            Phone = null,
            Fax = null
        },
        new Customer
        {
            CompanyName = "Tech Corp",
            ContactName = "Jane Doe",
            ContactTitle = "Manager",
            Address = "123 Main St",
            City = "Metropolis",
            Region = "NA",
            PostalCode = "12345",
            Country = "USA",
            Phone = "555-1234",
            Fax = "555-5678"
        }
    };

            // Act
            var transformedCustomers = _orderTransform.TransformCustomers(customers).ToList();

            // Assert
            var customer1 = transformedCustomers[0];
            Assert.Equal("N/A", customer1.CompanyName);
            Assert.Equal("N/A", customer1.ContactName);
            Assert.Equal("N/A", customer1.ContactTitle);
            Assert.Equal("N/A", customer1.Address);
            Assert.Equal("N/A", customer1.City);
            Assert.Equal("N/A", customer1.Region);
            Assert.Equal("N/A", customer1.PostalCode);
            Assert.Equal("N/A", customer1.Country);
            Assert.Equal("N/A", customer1.Phone);
            Assert.Equal("N/A", customer1.Fax);

            var customer2 = transformedCustomers[1];
            Assert.Equal("Tech Corp", customer2.CompanyName);
            Assert.Equal("Jane Doe", customer2.ContactName);
            Assert.Equal("Manager", customer2.ContactTitle);
            Assert.Equal("123 Main St", customer2.Address);
            Assert.Equal("Metropolis", customer2.City);
            Assert.Equal("N/A", customer2.Region); // Check cleaned string result
            Assert.Equal("12345", customer2.PostalCode);
            Assert.Equal("Usa", customer2.Country);
            Assert.Equal("5551234", customer2.Phone);
            Assert.Equal("5555678", customer2.Fax);
        }


    }
}

