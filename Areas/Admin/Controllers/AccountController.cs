    using HappyBakeryManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HappyBakeryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private const int PageSize = 10;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(string roleFilter, string searchName, int page = 1)
        {
            var accounts = await _accountService.GetAllAccountsAsync(roleFilter, searchName, page, PageSize);
            var total = await _accountService.GetTotalCountAsync(roleFilter, searchName);

            ViewBag.RoleFilter = roleFilter;
            ViewBag.SearchName = searchName;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)PageSize);

            return View(accounts);
        }

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(string email, string password, string role)
        {
            var (isSuccess, message) = await _accountService.CreateAccountAsync(email, password, role);
            TempData["msg"] = message;
            if (isSuccess)
                return RedirectToAction("Index");
            return View();
        }

        public async Task<IActionResult> Lock(string id)
        {
            await _accountService.LockAccountAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Unlock(string id)
        {
            await _accountService.UnlockAccountAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _accountService.DeleteAccountAsync(id);
            return RedirectToAction("Index");
        }
    }
}
