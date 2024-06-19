using ETL.Transform;
namespace ETL.UnitTests
{
    public class CleanStringTests
    {
        private readonly OrderTransform _orderTransform;

        public CleanStringTests()
        {
            _orderTransform = new OrderTransform();
        }

        [Theory]
        [InlineData(null, "N/A")]
        [InlineData("", "N/A")]
        [InlineData("  ", "N/A")]
        [InlineData("NA", "N/A")]
        [InlineData("n/a", "N/A")]
        [InlineData("Valid String", "Valid String")]
        [InlineData("VaLiD StRiNg", "Valid String")]
        [InlineData("  edge  ", "Edge")]
        [InlineData("special@chars!", "Specialchars")]
        public void CleanString_ShouldHandleVariousInputs(string input, string expected)
        {
            // Act
            var result = _orderTransform.CleanString(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
