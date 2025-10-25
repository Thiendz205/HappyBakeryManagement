namespace HappyBakeryManagement.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string? Note { get; set; }

        // 🔹 Hiển thị tên, nhưng vẫn lưu ID trong bảng chính
        public int PaymentMethodID { get; set; }
        public string PaymentMethodName { get; set; }

        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
    }
}
