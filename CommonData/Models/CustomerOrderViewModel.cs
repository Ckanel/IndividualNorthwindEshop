namespace CommonData.Models
{
    public class CustomerOrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerId { get; set; }
        public string CompanyName { get; set; }
        public string CustomerEmail { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }


}
