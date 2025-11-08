using System.Linq.Expressions;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAsync(Expression<Func<Customer, bool>>? predicate = null);
        Task<Customer?> GetByIdAsync(int id);
        Task<int> CreateAsync(Customer entity);
        Task UpdateAsync(Customer entity);
        Task DeleteAsync(int id); // throw nếu dính FK
        Task<bool> PhoneExistsAsync(string phone, int? ignoreId = null);
    }
}
