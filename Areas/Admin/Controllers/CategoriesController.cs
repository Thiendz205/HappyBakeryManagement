using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        public readonly ICategoriesServices _categoriesServices;
        public CategoriesController(ICategoriesServices categoriesServices)
        {
            _categoriesServices = categoriesServices;
        }

        public IActionResult Index()
        {
            var categories = _categoriesServices.getCategories();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(CategoriesDTO categoryDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _categoriesServices.AddCategory(categoryDto);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(categoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _categoriesServices.Detele(id);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var category = _categoriesServices.FindById(Id); 
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoriesDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryDto);
            }

            try
            {
                if (_categoriesServices.ExistsByName(categoryDto.Name, categoryDto.Id))
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại. Vui lòng chọn tên khác.");
                    return View(categoryDto);
                }

                var category = new HappyBakeryManagement.Models.Category
                {
                    Id = categoryDto.Id,
                    Name = categoryDto.Name,
                    Description = categoryDto.Description
                };

                _categoriesServices.UpdateCategory(category);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi khi cập nhật: {ex.Message}");
                return View(categoryDto);
            }
        }


    }
}
