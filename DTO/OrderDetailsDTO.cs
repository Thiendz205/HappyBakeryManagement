using System.ComponentModel.DataAnnotations;

namespace HappyBakeryManagement.DTO
{
    public class OrderDetailsDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đơn hàng")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }

        [Display(Name = "Tổng tiền")]
        [Range(1000, 100000000, ErrorMessage = "Tổng tiền phải hợp lệ (tối thiểu 1.000đ)")]
        public decimal ?TotalAmount { get; set; }

        // Dùng để hiển thị ra View
        public string? ProductName { get; set; }
        public string? OrderCode { get; set; }
        public string? CustomerName { get; set; }

    }
}
