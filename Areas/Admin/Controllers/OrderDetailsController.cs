using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HappyBakeryManagement.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderDetailsController : Controller
    {
        private readonly IOrderDetailsService _service;

        public OrderDetailsController(IOrderDetailsService service)
        {
            _service = service;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var all = _service.GetAll();
            int totalItems = all.Count;
            var paged = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(paged);
        }

        [HttpGet]
        public IActionResult Add()
        {
            // Lấy danh sách đơn hàng
            var orders = _service.GetAllOrders()
                .Select(o => new
                {
                    o.Id,
                    DisplayName = "Order#" + o.Id + " - " + o.Customer.FullName
                }).ToList();

            // Lấy danh sách sản phẩm
            var products = _service.GetAllProducts();

            // Gán cho ViewBag
            ViewBag.Orders = orders;
            ViewBag.Products = products;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(OrderDetailsDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (_service.Add(dto))
                    return RedirectToAction(nameof(Index));
            }

            // Render lại dropdown khi lỗi
            var orders = _service.GetAllOrders()
                .Select(o => new
                {
                    o.Id,
                    DisplayName = "Order#" + o.Id + " - " + o.Customer.FullName
                }).ToList();

            ViewBag.Orders = orders;
            ViewBag.Products = _service.GetAllProducts();

            return View(dto);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var dto = _service.FindById(id);
            if (dto == null)
                return NotFound();

            ViewBag.Orders = _service.GetAllOrders()
                .Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = "Order#" + o.Id + " - " + o.Customer.FullName
                }).ToList();

            ViewBag.Products = _service.GetAllProducts()
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(OrderDetailsDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Orders = _service.GetAllOrders()
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = "Order#" + o.Id + " - " + o.Customer.FullName
                    }).ToList();

                ViewBag.Products = _service.GetAllProducts()
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Name
                    }).ToList();

                return View(dto);
            }

            bool updated = _service.Update(dto);
            if (!updated)
                return NotFound();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
