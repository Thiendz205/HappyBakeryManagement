using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentMethodController : Controller
    {
        private readonly IPaymentMethodServices _service;

        public PaymentMethodController(IPaymentMethodServices service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var list = _service.GetAll();
            return View(list);
        }

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(PaymentMethodDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (_service.ExistsByName(dto.NamePaymentMethod))
                {
                    ModelState.AddModelError("NamePaymentMethod", "⚠️ Tên phương thức đã tồn tại!");
                    return View(dto);
                }
                var entity = new PaymentMethod
                {
                    NamePaymentMethod = dto.NamePaymentMethod
                };
                _service.Add(entity);
                return RedirectToAction("Index");
            }
            return View(dto);
        }

        public IActionResult Edit(int id)
        {
            var pm = _service.FindById(id);
            if (pm == null) return NotFound();

            var dto = new PaymentMethodDTO
            {
                Id = pm.Id,
                NamePaymentMethod = pm.NamePaymentMethod,
                OldName = pm.NamePaymentMethod 
            };
            return View(dto);
        }

        [HttpPost]
        public IActionResult Edit(PaymentMethodDTO dto)
        {
            if (ModelState.IsValid)
            {
                // 🔸 Nếu tên mới khác tên cũ thì mới cần kiểm tra trùng
                if (!dto.NamePaymentMethod.Trim().Equals(dto.OldName?.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    if (_service.ExistsByName(dto.NamePaymentMethod, dto.Id))
                    {
                        ModelState.AddModelError("NamePaymentMethod", "⚠️ Tên phương thức đã tồn tại!");
                        return View(dto);
                    }
                }

                var entity = new PaymentMethod
                {
                    Id = dto.Id,
                    NamePaymentMethod = dto.NamePaymentMethod.Trim()
                };
                _service.Update(entity);
                return RedirectToAction("Index");
            }
            return View(dto);
        }


        public IActionResult Delete(int id)
        {
            if (!_service.Delete(id))
            {
                TempData["Error"] = "❌ Không thể xóa! Có đơn hàng đang sử dụng phương thức này.";
            }
            return RedirectToAction("Index");
        }
    }
}
