using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
