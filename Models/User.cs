using System.Data;

namespace IndividualNorthwindEshop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
    }
    public enum Role
    {
        Customer,
        Admin,
        Manager
    }
}
