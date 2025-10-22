using System.ComponentModel.DataAnnotations;

namespace HappyBakeryManagement.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public string PhoneNumber {  get; set; }
        public string Address { get; set; }
        public string CitizenID { get; set; }
        public bool Gender { get; set; }
        public DateTime DOB { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public string? UserId { get; set; } // Nếu bạn vẫn muốn lưu thủ công Id user
        public virtual ApplicationUser? User { get; set; }

    }
}
