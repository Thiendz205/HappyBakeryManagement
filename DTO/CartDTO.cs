namespace HappyBakeryManagement.DTO
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public DateTime Evaluationdate { get; set; }
    }
}

