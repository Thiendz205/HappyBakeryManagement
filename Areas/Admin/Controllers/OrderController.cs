using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace HappyBakeryManagement.Controllers
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
        private readonly ApplicationDbContext db;
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
        public IActionResult Add(OrderDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(dto);
            }

            // ✅ Map DTO sang Model
            var order = new Order
            {
                BookingDate = dto.BookingDate,
                Status = dto.Status,
                DeliveryAddress = dto.DeliveryAddress,
                PhoneNumber = dto.PhoneNumber,
                Note = dto.Note,
                PaymentMethodID = dto.PaymentMethodID,
                CustomerID = dto.CustomerID
            };

            // ✅ Gọi service thay vì dùng db trực tiếp
            var success = _orderService.AddOrder(order);

            if (!success)
            {
                ModelState.AddModelError("", "Không thể thêm đơn hàng. Vui lòng thử lại.");
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(dto);
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _orderService.FindOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var dto = new OrderDTO
            {
                Id = order.Id,
                BookingDate = order.BookingDate,
                Status = order.Status,
                DeliveryAddress = order.DeliveryAddress,
                PhoneNumber = order.PhoneNumber,
                Note = order.Note,
                PaymentMethodID = order.PaymentMethodID,
                CustomerID = order.CustomerID
            };

            ViewBag.Customers = _orderService.GetAllCustomers();
            ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(dto);
            }

            // Kiểm tra ngày hợp lệ (nếu bạn muốn logic phía controller)
            if (dto.BookingDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("BookingDate", "Ngày đặt hàng không được nhỏ hơn hôm nay.");
                ViewBag.Customers = _orderService.GetAllCustomers();
                ViewBag.PaymentMethods = _orderService.GetAllPaymentMethods();
                return View(dto);
            }

            var order = new Order
            {
                Id = dto.Id,
                BookingDate = dto.BookingDate,
                Status = dto.Status,
                DeliveryAddress = dto.DeliveryAddress,
                PhoneNumber = dto.PhoneNumber,
                Note = dto.Note,
                PaymentMethodID = dto.PaymentMethodID,
                CustomerID = dto.CustomerID
            };

            bool updated = _orderService.UpdateOrder(order);
            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

    }
}
