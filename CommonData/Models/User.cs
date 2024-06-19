using Microsoft.AspNetCore.Identity;
using System.Data;

namespace CommonData.Models
{
    public class User : IdentityUser
    {

        public string? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
