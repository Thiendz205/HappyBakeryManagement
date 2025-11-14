namespace HappyBakeryManagement.Models
{
    public class CartItemVM
    {
        public int CartId { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}

