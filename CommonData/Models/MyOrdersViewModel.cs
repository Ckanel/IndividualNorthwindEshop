namespace CommonData.Models
{
    public class MyOrdersViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string?ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public string? GuestEmail { get; set; }
    }
}
