using Xunit;
using ETL.Transform;

namespace ETL.UnitTests
{
    public class HandleOrderDetailUnitPriceTests
    {
        private readonly OrderTransform _orderTransform;

        public HandleOrderDetailUnitPriceTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void HandleOrderDetailUnitPrice_ShouldHandleVariousInputs(decimal? input, decimal expected)
        {
            // Act
            var result = _orderTransform.HandleOrderDetailUnitPrice(input);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetTestData() => new List<object[]>
        {
            new object[] { (decimal?)20.5m, 20.5m },
            new object[] { (decimal?)0m, 0m },
            new object[] { (decimal?)null, 0m },
            new object[] { (decimal?)(-10), 0m } // Will log error and return 0
        };
    }
}

