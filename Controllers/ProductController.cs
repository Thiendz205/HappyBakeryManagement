using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productService;
        private readonly ICategoriesServices _categoryService;
        private readonly ILogger<ProductDTO> _logger;

        public ProductController(IProductServices productService, IWebHostEnvironment env, ICategoriesServices categoryService, ILogger<ProductDTO> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _logger = logger;
        }

        public IActionResult Index(int page = 1, int pageSize = 9)
        {
            var allProducts = _productService.GetAvailableProducts(); // trả về IEnumerable<ProductDTO>
            var totalProducts = allProducts.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

            var products = allProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = _categoryService.getCategories();

            return View(products);
        }

        public IActionResult Detail(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }



    }
}
