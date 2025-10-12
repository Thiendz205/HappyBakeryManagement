using Humanizer.Localisation;

namespace HappyBakeryManagement.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Detail { get; set; }
        public decimal Price { get; set; } = 0;
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; private set; }
        public int ? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

    }
}