// Controllers/CustomerController.cs
using HappyBakeryManagement.Data;
using HappyBakeryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db; _userManager = userManager;
    }

    [HttpGet]
    public IActionResult CreateProfile()
    {
        // Trả rõ đường dẫn tuyệt đối đến view
        return View("~/Views/Customer/CreateProfile.cshtml");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProfile(Customer model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        if (!ModelState.IsValid)
            return View("~/Views/Customer/CreateProfile.cshtml", model);

        var existed = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == user.Id);
        if (existed == null)
        {
            model.UserId = user.Id;              
            _db.Customers.Add(model);
        }
        else
        {
            existed.FullName = model.FullName;
            existed.PhoneNumber = model.PhoneNumber;
            existed.Address = model.Address;
            existed.CitizenID = model.CitizenID;
            existed.Gender = model.Gender;
            existed.DOB = model.DOB;
            _db.Customers.Update(existed);
        }

        await _db.SaveChangesAsync();
        TempData["ok"] = "Hoàn thiện hồ sơ thành công!";
        return RedirectToAction("Index", "Home");
    }

}
