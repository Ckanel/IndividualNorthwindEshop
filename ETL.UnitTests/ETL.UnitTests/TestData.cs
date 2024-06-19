public class HandleProductUnitPriceTestData
{
    public static IEnumerable<object[]> TestData =>
        new List<object[]>
        {
            new object[] { (decimal?)20.5m, 20.5m },
            new object[] { (decimal?)0m, 0m },
            new object[] { (decimal?)null, 0m },
            new object[] { (decimal?)(-10), 0m } 
        };
}

