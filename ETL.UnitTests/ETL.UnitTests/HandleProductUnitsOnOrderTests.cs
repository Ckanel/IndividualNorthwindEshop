using ETL.Transform;
namespace ETL.UnitTests
{
    public class HandleProductUnitsOnOrderTests
    {
        private readonly OrderTransform _orderTransform;

        public HandleProductUnitsOnOrderTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [InlineData((short)500, (short)500)]
        [InlineData((short)-5, (short)0)] // Negative handled to zero
        [InlineData((short)1001, (short)1000)] // Capped to 1000
        [InlineData(null, (short)0)]
        public void HandleProductUnitsOnOrder_ShouldHandleVariousInputs(short? input, short expected)
        {
            // Act
            var result = _orderTransform.HandleProductUnitsOnOrder(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }

}