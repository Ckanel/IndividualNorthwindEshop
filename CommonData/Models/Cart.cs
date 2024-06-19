using System.ComponentModel.DataAnnotations;

namespace CommonData.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        [MaxLength(5)]
        public string CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}
