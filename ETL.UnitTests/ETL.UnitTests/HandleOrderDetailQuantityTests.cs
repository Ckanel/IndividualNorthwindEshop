using Xunit;
using ETL.Transform;

namespace ETL.UnitTests
{
    public class HandleOrderDetailQuantityTests
    {
        private readonly OrderTransform _orderTransform;

        public HandleOrderDetailQuantityTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void HandleOrderDetailQuantity_ShouldHandleVariousInputs(short? input, short expected)
        {
            // Act
            var result = _orderTransform.HandleOrderDetailQuantity(input);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetTestData() => new List<object[]>
        {
            new object[] { (short?)50, (short)50 },
            new object[] { (short?)(-1), (short)0 }, // Negative handled to zero
            new object[] { (short?)null, (short)0 }
        };
    }
}

