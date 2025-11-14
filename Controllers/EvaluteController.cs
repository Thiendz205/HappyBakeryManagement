using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Controllers
{
    [Authorize]
    public class EvaluteController : Controller
    {
        private readonly IEvaluteServices _evaluteServices;
        public EvaluteController(IEvaluteServices evaluteServices)
        {
            _evaluteServices = evaluteServices;
        }
        public async Task<IActionResult> Index(string productName)
        {
            var feedbacks = await _evaluteServices.GetAllEvalutesWithInfoAsync();

            if (!string.IsNullOrWhiteSpace(productName))
            {
                feedbacks = feedbacks
                    .Where(f => f.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.CurrentFilter = productName;
            return View(feedbacks);
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            return View(new EvaluteDTO { ProductID = id });
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Add")]
        public async Task<IActionResult> AddPost([Bind("ProductID,Score")] EvaluteDTO dto)
        {
            if (!ModelState.IsValid) return View("Add", dto);

            var (ok, err) = await _evaluteServices.AddEvaluteAsync(dto, User);
            if (ok) { TempData["success"] = "Cảm ơn bạn đã đánh giá!"; return RedirectToAction("Index", "Evalute"); }
            TempData["error"] = err ?? "Thêm đánh giá thất bại.";
            return View("Add", dto);
        }


    }
}
