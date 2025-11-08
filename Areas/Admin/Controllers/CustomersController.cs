using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class CustomersController : Controller
    {
        private readonly ICustomerService _svc;
        public CustomersController(ICustomerService svc)
        {
            _svc = svc;
        }

        // 📋 Danh sách khách hàng
        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAsync();
            return View(list);
        }

        // ➕ Form thêm khách hàng
        public IActionResult Add() => View(new Customer());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Customer model)
        {
            if (!ModelState.IsValid) return View(model);
            await _svc.CreateAsync(model);
            TempData["ok"] = "Thêm khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ✏️ Sửa khách hàng
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _svc.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return View(entity);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer model)
        {
            if (!ModelState.IsValid) return View(model);
            await _svc.UpdateAsync(model);
            TempData["ok"] = "Cập nhật khách hàng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ❌ Xóa khách hàng
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                TempData["ok"] = "Xóa khách hàng thành công!";
            }
            catch (DbUpdateException)
            {
                TempData["err"] = "Không thể xóa vì khách hàng đang được tham chiếu!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
