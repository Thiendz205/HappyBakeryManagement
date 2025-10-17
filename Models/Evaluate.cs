namespace HappyBakeryManagement.Models
{
    public class Evaluate
    {
        public int Id {  get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Score { get; set; }
        public DateTime Evaluationdate { get; set; }
    }
}
