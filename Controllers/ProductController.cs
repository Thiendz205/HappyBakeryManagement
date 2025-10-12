using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
