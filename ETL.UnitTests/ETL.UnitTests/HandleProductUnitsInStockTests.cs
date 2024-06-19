using ETL.Transform;
namespace ETL.UnitTests
{
    public class HandleProductUnitsInStockTests
    {
        private readonly OrderTransform _orderTransform;

        public HandleProductUnitsInStockTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [InlineData((short)50, (short)50)]
        [InlineData((short)-1, (short)0)] // Negative handled to zero
        [InlineData(null, (short)0)]
        public void HandleProductUnitsInStock_ShouldHandleVariousInputs(short? input, short expected)
        {
            // Act
            var result = _orderTransform.HandleProductUnitsInStock(input, null);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
