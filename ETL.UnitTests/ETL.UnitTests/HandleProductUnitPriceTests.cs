using Xunit;
using ETL.Transform;

namespace ETL.UnitTests
{
    public class HandleProductUnitPriceTests
    {
        private readonly OrderTransform _orderTransform;

        public HandleProductUnitPriceTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [MemberData(nameof(HandleProductUnitPriceTestData.TestData), MemberType = typeof(HandleProductUnitPriceTestData))]
        public void HandleProductUnitPrice_ShouldHandleVariousInputs(decimal? input, decimal expected)
        {
            // Act
            var result = _orderTransform.HandleProductUnitPrice(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}

