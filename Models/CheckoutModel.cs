using System.ComponentModel.DataAnnotations;

namespace IndividualNorthwindEshop.Models
{
    public class CheckoutModel
    {
        public string CustomerName { get; set; }
        [MaxLength(100)]
        [EmailAddress]
        public string GuestEmail { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
