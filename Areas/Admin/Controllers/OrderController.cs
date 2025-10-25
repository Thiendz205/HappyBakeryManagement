using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderServices _orderService;

        public OrderController(IOrderServices orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 20;
            var orders = _orderService.GetOrdersPaged(page, pageSize);
            int totalOrders = _orderService.GetTotalOrders();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return View(orders);
        }
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Customers = _orderService.GetAllCustomers();
            ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Order order)
        {
            // Kiểm tra model có hợp lệ không
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(order);
            }

            // Lấy lại customer và payment method theo ID
            var customer = _orderService.GetAllCustomers().FirstOrDefault(c => c.Id == order.CustomerID);
            var payment = _orderService.GetAllPaymentMethods().FirstOrDefault(p => p.Id == order.PaymentMethodID);

            if (customer == null || payment == null)
            {
                ModelState.AddModelError("", "Khách hàng hoặc phương thức thanh toán không tồn tại.");
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(order);
            }

            order.Customer = customer;
            order.PaymentMethod = payment;
            order.BookingDate = DateTime.Now;
            order.OrderDetailss = new List<OrderDetails>();

            // Gọi BLL (Service Layer)
            bool added = _orderService.AddOrder(order);

            if (!added)
            {
                ModelState.AddModelError("", "Thêm đơn hàng thất bại. Vui lòng thử lại.");
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(order);
            }

            return RedirectToAction("Index");
        }
    }
}
