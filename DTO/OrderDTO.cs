using System.ComponentModel.DataAnnotations;

namespace HappyBakeryManagement.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        [Required(ErrorMessage = "Vui lòng chọn ngày đặt hàng.")]
        [DateNotInPast]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Trạng thái")]
        [Required(ErrorMessage = "Vui lòng nhập trạng thái.")]
        public string Status { get; set; }

        [Display(Name = "Địa chỉ giao hàng")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^(0[0-9]{9})$", ErrorMessage = "Số điện thoại không hợp lệ (phải có 10 số và bắt đầu bằng 0).")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        // 🔹 ID là bắt buộc vì để lưu
        [Display(Name = "Phương thức thanh toán")]
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public int PaymentMethodID { get; set; }

        // 🔹 Tên chỉ hiển thị, cho phép null
        public string? PaymentMethodName { get; set; }

        [Display(Name = "Khách hàng")]
        [Required(ErrorMessage = "Vui lòng chọn khách hàng.")]
        public int CustomerID { get; set; }

        public string? CustomerName { get; set; }
        public class DateNotInPastAttribute : ValidationAttribute
        {
            public DateNotInPastAttribute()
            {
                ErrorMessage = "Ngày đặt hàng không được nhỏ hơn ngày hôm nay.";
            }

            public override bool IsValid(object? value)
            {
                if (value == null) return true; // để [Required] xử lý riêng
                if (value is DateTime date)
                {
                    return date.Date >= DateTime.Now.Date;
                }
                return false;
            }
        }
    }
}
