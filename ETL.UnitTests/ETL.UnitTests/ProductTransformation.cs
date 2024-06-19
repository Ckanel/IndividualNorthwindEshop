using Xunit;
using ETL.Transform;
using CommonData.Models;
using System.Collections.Generic;
using System.Linq;

namespace ETL.UnitTests
{
    public class ProductTransformationTests
    {
        private readonly OrderTransform _orderTransform;

        public ProductTransformationTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Fact]
        public void TransformProducts_ShouldTransformProductsCorrectly()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    ProductName = null,
                    QuantityPerUnit = null,
                    UnitPrice = 20.5m,
                    UnitsInStock = 50,
                    ReorderLevel = 20,
                    UnitsOnOrder = 5,
                    Discontinued = false
                },
                new Product
                {
                    ProductName = "Test Product",
                    QuantityPerUnit = "10 units",
                    UnitPrice = -10, // invalid price: expected to be defaulted to zero
                    UnitsInStock = 10,
                    ReorderLevel = 5,
                    UnitsOnOrder = 1001, // should be capped
                    Discontinued = true
                }
            };

            // Act
            var transformedProducts = _orderTransform.TransformProducts(products).ToList();

            // Assert
            var product1 = transformedProducts[0];
            Assert.Equal("N/A", product1.ProductName);
            Assert.Equal("N/A", product1.QuantityPerUnit);
            Assert.Equal(20.5m, product1.UnitPrice); // verifying valid value
            Assert.Equal((short)50, product1.UnitsInStock);
            Assert.Equal((short)5, product1.UnitsOnOrder);
            Assert.False(product1.Discontinued);

            var product2 = transformedProducts[1];
            Assert.Equal("Test Product", product2.ProductName);
            Assert.Equal("10 Units", product2.QuantityPerUnit);
            Assert.Equal(0, product2.UnitPrice);  // corrected invalid price to zero
            Assert.Equal((short)10, product2.UnitsInStock);
            Assert.Equal((short)1000, product2.UnitsOnOrder); // capped to valid value
            Assert.True(product2.Discontinued);
        }
    }
}

