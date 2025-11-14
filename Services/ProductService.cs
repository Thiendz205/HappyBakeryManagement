using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Repository
{
    public class ProductService : IProductServices
    {
        private readonly ApplicationDbContext _db;

        public ProductService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ Lấy danh sách tất cả sản phẩm
        public List<ProductDTO> GetAllProducts()
        {
            var products = _db.Products
                .Include(p => p.Category)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Detail = p.Detail,
                    Price = p.Price,
                    Quantity = p.Quantity, // ✅ thêm số lượng
                    CategoryID = p.Category.Id,
                    CategoryName = p.Category.Name,
                    Image = p.Image
                });

            return products.ToList();
        }

        // ✅ Kiểm tra trùng tên
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Products.AnyAsync(p => p.Name.ToLower() == name.ToLower());
        }

        // ✅ Thêm sản phẩm mới
        public async Task AddAsync(ProductDTO productDTO)
        {
            // Kiểm tra tên trùng (bỏ khoảng trắng và thường hóa)
            var existing = await _db.Products
                .FirstOrDefaultAsync(p => p.Name.Trim().ToLower() == productDTO.Name.Trim().ToLower());

            if (existing != null)
            {
                // ✅ Nếu trùng: tăng số lượng
                existing.Quantity += productDTO.Quantity;

                // (Tuỳ chọn) cập nhật thêm thông tin
                existing.Price = productDTO.Price;
                existing.Detail = productDTO.Detail;
                existing.Image = productDTO.Image;
                existing.CategoryId = productDTO.CategoryID;

                _db.Products.Update(existing);
            }
            else
            {
                // ✅ Nếu chưa có thì tạo mới
                var newProduct = new Product
                {
                    Name = productDTO.Name.Trim(),
                    Detail = productDTO.Detail,
                    Price = productDTO.Price,
                    Quantity = productDTO.Quantity,
                    CategoryId = productDTO.CategoryID,
                    Image = productDTO.Image
                };

                _db.Products.Add(newProduct);
            }

            await _db.SaveChangesAsync();
        }


        // ✅ Lấy sản phẩm theo Id
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
                    Quantity = p.Quantity, // ✅ thêm số lượng
                    CategoryID = p.Category.Id,
                    CategoryName = p.Category.Name,
                    Image = p.Image
                })
                .FirstOrDefault();

            return product;
        }

        // ✅ Xóa sản phẩm
        public void DeleteProduct(int id)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
        }

        // ✅ Cập nhật sản phẩm
        public void UpdateProduct(ProductDTO dto)
        {
            var p = _db.Products.FirstOrDefault(x => x.Id == dto.Id);
            if (p == null)
                throw new Exception("Không tìm thấy sản phẩm.");

            p.Name = dto.Name;
            p.Detail = dto.Detail;
            p.Price = dto.Price;
            p.Quantity = dto.Quantity; // ✅ cập nhật số lượng
            p.CategoryId = dto.CategoryID;
            p.Image = dto.Image;

            _db.Products.Update(p);
            _db.SaveChanges();
        }

        public List<ProductDTO> GetAvailableProducts()
        {
            return _db.Products
                .Include(p => p.Category)
                .Where(p => p.Quantity > 0) // ❗ Chỉ lấy sản phẩm còn hàng
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Detail = p.Detail,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryID = p.Category.Id,
                    CategoryName = p.Category.Name,
                    Image = p.Image
                })
                .ToList();
        }

    }
}
