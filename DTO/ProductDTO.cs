namespace HappyBakeryManagement.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Detail { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Image { get; set; }

        public IFormFile? ImageFile { get; set; }

    }
}
