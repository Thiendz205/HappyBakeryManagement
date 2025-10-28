using System.ComponentModel.DataAnnotations;

namespace HappyBakeryManagement.DTO
{
    public class PaymentMethodDTO
    {
        public int Id { get; set; }

        [Display(Name = "Tên phương thức thanh toán")]
        [Required(ErrorMessage = "Vui lòng nhập tên phương thức thanh toán.")]
        public string NamePaymentMethod { get; set; }
        public string? OldName { get; set; }
    }
}
