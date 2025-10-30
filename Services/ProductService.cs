using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Repository
{
    public class ProductService: IProductServices
    {
        public readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }   
        public List<ProductDTO> GetAllProducts()
        {
            var products = from p in _db.Products
                            .Include(p => p.Category)
                           select new ProductDTO
                           {
                               Id = p.Id,
                               Name = p.Name,
                               Detail = p.Detail,
                               Price = p.Price,
                               CreatedDate = p.CreatedDate,
                               EndDate = p.EndDate,
                               CategoryID = p.Category.Id,
                               CategoryName = p.Category.Name,
                               Image = p.Image
                           };
            return products.ToList();
        }
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task AddAsync(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Detail = productDTO.Detail,
                Price = productDTO.Price,
                CreatedDate = productDTO.CreatedDate,
                EndDate = productDTO.EndDate,
                CategoryId = productDTO.CategoryID,
                Image = productDTO.Image
            };

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public ProductDTO GetProductById(int id)
        {
            return _db.Products
                .Where(p => p.Id == id)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Image = p.Image
                }).FirstOrDefault();
        }

        public void DeleteProduct(int id)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }

    }
}
