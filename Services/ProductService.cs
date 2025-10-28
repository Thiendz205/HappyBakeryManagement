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
                               CategoryName = p.Name,
                               Image = p.Image
                           };
            return products.ToList();
        }

        
    }
}
