using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EvaluteController : Controller
    {
        public readonly IEvaluteServices _evaluteServices;
        public EvaluteController(IEvaluteServices evaluteServices)
        {
            _evaluteServices =  evaluteServices;
        }
        public IActionResult Index()
        {
            var evalutes = _evaluteServices.getAllEvalute();
            return View(evalutes);
        }
    }
}
