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
            var product = _db.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductDTO
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
                })
                .FirstOrDefault();

            return product;
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

        public void UpdateProduct(ProductDTO dto)
        {
            var p = _db.Products.FirstOrDefault(x => x.Id == dto.Id);
            if (p == null) throw new Exception("Không tìm thấy sản phẩm.");

            p.Name = dto.Name;
            p.Detail = dto.Detail;
            p.Price = dto.Price;
            p.CreatedDate = dto.CreatedDate;
            p.EndDate = dto.EndDate;
            p.CategoryId = dto.CategoryID;
            p.Image = dto.Image;

            _db.Products.Update(p);
            _db.SaveChanges();
        }


    }
}
