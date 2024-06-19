using Xunit;
using ETL.Transform;

namespace ETL.UnitTests
{
    public class CalculateOrderDetailTotalTests
    {
        private readonly OrderTransform _orderTransform;

        public CalculateOrderDetailTotalTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [MemberData(nameof(CalculateOrderDetailTotalTestData.Data), MemberType = typeof(CalculateOrderDetailTotalTestData))]
        public void CalculateOrderDetailTotal_ShouldReturnCorrectTotal(decimal unitPrice, short quantity, float discount, decimal expected)
        {
            // Act
            var result = _orderTransform.CalculateOrderDetailTotal(unitPrice, quantity, discount);

            // Assert
            Assert.Equal(expected, result);
        }
    }
    public static class CalculateOrderDetailTotalTestData
    {
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
            new object[] { 10m, (short)2, 0.1f, 18m },
            new object[] { 5m, (short)3, 0.2f, 12m },
            new object[] { 0m, (short)5, 0.3f, 0m }
            };
    }
}

