using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HappyBakeryManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class OrdersController : Controller
    {
        private readonly IOrderServices _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(IOrderServices orderService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // 🧾 Hiển thị form đặt hàng
        [HttpGet]
        public IActionResult Checkout()
        {
            ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
            return View();
        }

        // 🛒 Xử lý đặt hàng (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string deliveryAddress, string phoneNumber, string? note, int paymentMethodId)
        {
            var user = await _userManager.GetUserAsync(User);

            // ✅ Kiểm tra nếu chưa có thông tin khách hàng
            if (user?.CustomerId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần cập nhật thông tin khách hàng trước khi đặt hàng.";
                return RedirectToAction("Profile", "Customer");
            }

            try
            {
                int customerId = user.CustomerId.Value;

                bool success = _orderService.PlaceOrder(customerId, deliveryAddress, phoneNumber, note, paymentMethodId);

                if (success)
                {
                    TempData["SuccessMessage"] = "Đặt hàng thành công!";
                    return RedirectToAction("OrderSuccess");
                }

                TempData["ErrorMessage"] = "Không thể đặt hàng. Vui lòng thử lại.";
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View();
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi đặt hàng: " + ex.Message;
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View();
            }
        }

        // ✅ Trang hiển thị khi đặt hàng thành công
        public IActionResult OrderSuccess()
        {
            return View();
        }

        // 📝 Danh sách đơn hàng của user
        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.CustomerId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có thông tin khách hàng, vui lòng cập nhật trước khi xem lịch sử đơn hàng.";
                return View("OrderHistory", new List<HappyBakeryManagement.DTO.OrderDTO>());
            }

            int customerId = user.CustomerId.Value;
            var orders = _orderService.GetOrdersByCustomer(customerId);

            return View(orders);
        }

        // 📝 Chi tiết đơn hàng
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user?.CustomerId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần có thông tin khách hàng để xem chi tiết đơn hàng.";
                return RedirectToAction("Profile", "Customer");
            }

            int customerId = user.CustomerId.Value;

            var order = _orderService.GetOrderById(id);
            if (order == null || order.CustomerID != customerId)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại hoặc không thuộc về bạn.";
                return RedirectToAction("OrderHistory");
            }

            var orderDetails = _orderService.GetOrderDetailsByOrderId(id);
            ViewBag.Order = order;
            return View(orderDetails);
        }

    }
}
