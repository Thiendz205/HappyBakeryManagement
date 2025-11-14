using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HappyBakeryManagement.Services
{
    public class EvaluteServices: IEvaluteServices
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public readonly ApplicationDbContext _db;
        public EvaluteServices(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public List<EvaluteDTO> getAllEvalute()
        {
            var Evalute = from c in _db.Evaluates
                             select new EvaluteDTO
                             {
                                 Id = c.Id,
                                 CustomerID = c.Customer.Id,
                                 ProductID = c.Product.Id,
                                 Score = c.Score,
                                 Evaluationdate = c.Evaluationdate
                             };
            return Evalute.ToList();
        }

        public async Task<List<EvaluteViewModel>> GetAllEvalutesWithInfoAsync()
        {
            return await _db.Evaluates
                .Include(e => e.Customer)
                .Include(e => e.Product)
                .OrderByDescending(e => e.Evaluationdate)
                .Select(e => new EvaluteViewModel
                {
                    CustomerName = e.Customer.FullName,
                    ProductName = e.Product.Name,
                    Score = e.Score,
                    EvaluationDate = e.Evaluationdate
                })
                .ToListAsync();
        }


        public List<EvaluteDTO> GetEvalutesByProductId(int productId)
        {
            var evalutes = _db.Evaluates
                .Where(e => e.Product.Id == productId)
                .Select(e => new EvaluteDTO
                {
                    Id = e.Id,
                    CustomerID = e.Customer.Id,
                    ProductID = e.Product.Id,
                    Score = e.Score,
                    Evaluationdate = e.Evaluationdate
                })
                .ToList();

            return evalutes;
        }

        public async Task<(bool ok, string? error)> AddEvaluteAsync(EvaluteDTO dto, ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            if (string.IsNullOrEmpty(userId)) return (false, "Chưa đăng nhập.");

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                var appUser = await _userManager.FindByIdAsync(userId);

                customer = new Customer
                {
                    UserId = userId,
                    FullName = appUser?.UserName ?? "Khách hàng mới",
                    PhoneNumber = appUser?.PhoneNumber ?? "0000000000",
                    Address = "Đang cập nhật",
                    CitizenID = "000000000000",
                    Gender = true,
                    DOB = DateTime.UtcNow.AddYears(-20) 
                };

                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
            }

            var exists = await _db.Products.AnyAsync(p => p.Id == dto.ProductID);
            if (!exists) return (false, $"Sản phẩm #{dto.ProductID} không tồn tại.");

            if (dto.Score < 1 || dto.Score > 5)
                return (false, "Điểm phải từ 1–5.");

            var dup = await _db.Evaluates.AnyAsync(e => e.CustomerId == customer.Id && e.ProductId == dto.ProductID);
            if (dup) return (false, "Bạn đã đánh giá sản phẩm này.");

            var evaluate = new Evaluate
            {
                CustomerId = customer.Id,
                ProductId = dto.ProductID,
                Score = dto.Score,
                Evaluationdate = DateTime.UtcNow
            };

            _db.Evaluates.Add(evaluate);
            await _db.SaveChangesAsync();
            return (true, null);
        }







    }
}
