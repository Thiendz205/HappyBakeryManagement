namespace HappyBakeryManagement.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public DateTime Evaluationdate { get; set; }
    }
}
