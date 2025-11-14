using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;

        public CartService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> AddToCartAsync(int customerId, int productId, int quantity = 1)
        {
            // Kiểm tra sản phẩm có tồn tại không
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return false;

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var existingCart = await _db.Carts
                .FirstOrDefaultAsync(c => c.CustomerID == customerId && c.ProductId == productId);

            if (existingCart != null)
            {
                // Nếu đã có, cập nhật số lượng
                existingCart.Quantity += quantity;
                existingCart.Evaluationdate = DateTime.Now;
            }
            else
            {
                // Nếu chưa có, thêm mới
                var cart = new Cart
                {
                    CustomerID = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    Evaluationdate = DateTime.Now
                };
                _db.Carts.Add(cart);
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public List<CartDTO> GetCartByCustomerId(int customerId)
        {
            var query = from c in _db.Carts
                        .Include(c => c.Product)
                        .Include(c => c.Customer)
                        where c.CustomerID == customerId
                        select new CartDTO
                        {
                            Id = c.Id,
                            CustomerID = c.CustomerID,
                            CustomerName = c.Customer.FullName,
                            ProductId = c.ProductId,
                            ProductName = c.Product.Name,
                            ProductImage = c.Product.Image,
                            ProductPrice = c.Product.Price,
                            Quantity = c.Quantity,
                            Evaluationdate = c.Evaluationdate
                        };
            return query.ToList();
        }

        public async Task<bool> UpdateQuantityAsync(int cartId, int quantity)
        {
            if (quantity < 1) return false;

            var cart = await _db.Carts.FindAsync(cartId);
            if (cart == null) return false;

            cart.Quantity = quantity;
            cart.Evaluationdate = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            var cart = await _db.Carts.FindAsync(cartId);
            if (cart == null) return false;

            _db.Carts.Remove(cart);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int customerId)
        {
            var carts = await _db.Carts
                .Where(c => c.CustomerID == customerId)
                .ToListAsync();

            if (!carts.Any()) return false;

            _db.Carts.RemoveRange(carts);
            await _db.SaveChangesAsync();
            return true;
        }

        public decimal GetCartTotal(int customerId)
        {
            var total = _db.Carts
                .Include(c => c.Product)
                .Where(c => c.CustomerID == customerId)
                .Sum(c => c.Product.Price * c.Quantity);

            return total;
        }

        public int GetCartItemCount(int customerId)
        {
            return _db.Carts
                .Where(c => c.CustomerID == customerId)
                .Sum(c => c.Quantity);
        }
    }
}

