namespace HappyBakeryManagement.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        public int ProductID { get; set; }
        public virtual Product  Product { get; set; }
    }
}
