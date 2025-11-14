namespace HappyBakeryManagement.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Detail { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public string? Image { get; set; }

        public IFormFile? ImageFile { get; set; }

    }
}
