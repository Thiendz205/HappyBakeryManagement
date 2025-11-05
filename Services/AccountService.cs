using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using Microsoft.AspNetCore.Identity;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;
namespace HappyBakeryManagement.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<AccountDTO>> GetAllAccountsAsync(string roleFilter = null, string searchName = null, int page = 1, int pageSize = 20)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.UserName.Contains(searchName));

            var users = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var result = new List<AccountDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roleFilter == null || roles.Contains(roleFilter))
                {
                    result.Add(new AccountDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Role = roles.FirstOrDefault(),
                        IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now
                    });
                }
            }

            return result;
        }

        public async Task<int> GetTotalCountAsync(string roleFilter = null, string searchName = null)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(u => u.UserName.Contains(searchName));

            return await query.CountAsync();
        }

        public async Task<(bool isSuccess, string message)> CreateAccountAsync(string email, string password, string role)
        {
            // ✅ Check trùng email
            var existingEmail = await _userManager.FindByEmailAsync(email);
            if (existingEmail != null)
                return (false, "Email này đã được sử dụng.");

            var user = new ApplicationUser
            {
                UserName = email, // Dùng email làm username
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return (true, "Tạo tài khoản thành công!");
        }


        public async Task LockAccountAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task UnlockAccountAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task DeleteAccountAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }
    }
}
 