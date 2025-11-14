using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _cartService = cartService;
            _userManager = userManager;
            _db = db;
        }

        // Lấy CustomerId từ User hiện tại, tự động tạo Customer nếu chưa có
        private async Task<int?> GetCurrentCustomerIdAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;

            // Nếu đã có CustomerId, trả về
            if (user.CustomerId != null)
            {
                return user.CustomerId;
            }

            // Nếu chưa có Customer, tự động tạo một Customer mới
            var customer = new Customer
            {
                FullName = user.UserName ?? user.Email ?? "Khách hàng",
                PhoneNumber = user.PhoneNumber ?? "",
                Address = "Chưa cập nhật",
                CitizenID = "Chưa cập nhật",
                UserId = user.Id,
                Gender = true
            };

            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            // Cập nhật CustomerId cho User
            user.CustomerId = customer.Id;
            await _userManager.UpdateAsync(user);

            return customer.Id;
        }

        public async Task<IActionResult> Index()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
            {
                // Nếu không lấy được user, redirect về login
                TempData["error"] = "Vui lòng đăng nhập để xem giỏ hàng.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var cartItems = _cartService.GetCartByCustomerId(customerId.Value);
            var total = _cartService.GetCartTotal(customerId.Value);

            // Chuyển đổi CartDTO sang CartItemVM
            var viewModel = cartItems.Select(c => new CartItemVM
            {
                CartId = c.Id,
                Name = c.ProductName ?? "",
                Image = c.ProductImage,
                Price = c.ProductPrice,
                Quantity = c.Quantity
            }).ToList();

            ViewBag.Total = total;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart([FromForm] int id, [FromForm] int quantity = 1)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
            {
                TempData["error"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (quantity < 1) quantity = 1;

            var result = await _cartService.AddToCartAsync(customerId.Value, id, quantity);
            if (result)
            {
                TempData["ok"] = "Đã thêm sản phẩm vào giỏ hàng!";
            }
            else
            {
                TempData["error"] = "Không thể thêm sản phẩm vào giỏ hàng.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQty([FromForm] int cartId, [FromForm] int qty)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập." });

            var cart = await _db.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId && c.CustomerID == customerId.Value);

            if (cart == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });

            if (qty < 1) qty = 1;

            var result = await _cartService.UpdateQuantityAsync(cartId, qty);
            if (!result)
                return Json(new { success = false, message = "Không thể cập nhật số lượng." });

            // lấy lại item & tổng mới nhất
            var updated = await _db.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            var subTotal = (updated?.Product.Price ?? 0m) * (updated?.Quantity ?? qty);
            var total = _cartService.GetCartTotal(customerId.Value);

            return Json(new
            {
                success = true,
                subTotal,  
                total       
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove([FromForm] int cartId)
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
            {
                TempData["error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Index");
            }

            // Kiểm tra cart thuộc về customer hiện tại
            var cart = await _db.Carts
                .FirstOrDefaultAsync(c => c.Id == cartId && c.CustomerID == customerId.Value);

            if (cart == null)
            {
                TempData["error"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
                return RedirectToAction("Index");
            }

            var result = await _cartService.RemoveFromCartAsync(cartId);
            if (result)
            {
                TempData["ok"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }
            else
            {
                TempData["error"] = "Không thể xóa sản phẩm.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var customerId = await GetCurrentCustomerIdAsync();
            if (customerId == null)
            {
                TempData["error"] = "Vui lòng đăng nhập.";
                return RedirectToAction("Index");
            }

            var result = await _cartService.ClearCartAsync(customerId.Value);
            if (result)
            {
                TempData["ok"] = "Đã xóa toàn bộ giỏ hàng!";
            }
            else
            {
                TempData["error"] = "Giỏ hàng đã trống.";
            }

            return RedirectToAction("Index");
        }
    }
}

