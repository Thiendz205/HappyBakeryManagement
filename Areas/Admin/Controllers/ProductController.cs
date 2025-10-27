using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        public readonly IProductServices _productService;
        public ProductController(IProductServices productService)
        {
            _productService = productService;   
        }

        public IActionResult Index()
        {
            var _products = _productService.GetAllProducts();
            return View(_products);
        }
        public IActionResult Add()
        {
            return View();
        }
    }
}
