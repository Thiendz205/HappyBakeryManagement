using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
