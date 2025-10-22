using Microsoft.AspNetCore.Identity;

namespace HappyBakeryManagement.Models
{
    public class ApplicationUser : IdentityUser 
    {
        public int? CustomerId { get; set; } // liên kết tới khách hàng (nếu là khách)
        public virtual Customer? Customer { get; set; }
    }
}
