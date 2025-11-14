using System.Diagnostics;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Repository;

namespace HappyBakeryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductServices _productService;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoriesServices _categoryService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductServices productService, IWebHostEnvironment env, ICategoriesServices categoryService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _env = env;
            _categoryService = categoryService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var availableProducts = _productService.GetAvailableProducts();
            return View(availableProducts);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
