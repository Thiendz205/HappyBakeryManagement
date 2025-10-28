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
    }
}
