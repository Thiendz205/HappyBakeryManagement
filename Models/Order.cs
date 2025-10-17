namespace HappyBakeryManagement.Models
{
    public class Order
    { 
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string ? Note { get; set; }
        public int PaymentMethodID { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderDetails> OrderDetailss { get; set; } = new List<OrderDetails>();
    }
}
