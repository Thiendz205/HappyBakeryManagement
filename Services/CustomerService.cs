using HappyBakeryManagement.Data;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HappyBakeryManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        public CustomerService(ApplicationDbContext db) => _db = db;
        public async Task<List<Customer>> GetAsync(Expression<Func<Customer, bool>>? predicate = null)
        {
            IQueryable<Customer> q = _db.Customers
                .Include(c => c.User)   
                .AsNoTracking();

            if (predicate != null) q = q.Where(predicate);

            return await q.OrderBy(x => x.Id).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
            => await _db.Customers
                .Include(c => c.User) 
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<int> CreateAsync(Customer e)
        {
            _db.Customers.Add(e);
            await _db.SaveChangesAsync();
            return e.Id;
        }

        public async Task UpdateAsync(Customer e)
        {
            var existed = await _db.Customers.FirstOrDefaultAsync(x => x.Id == e.Id);
            if (existed == null) return;

            existed.FullName = e.FullName;
            existed.PhoneNumber = e.PhoneNumber;
            existed.Address = e.Address;
            existed.CitizenID = e.CitizenID;
            existed.Gender = e.Gender;
            existed.DOB = e.DOB;


            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Customers.FindAsync(id);
            if (entity is null) return;
            _db.Customers.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public Task<bool> PhoneExistsAsync(string phone, int? ignoreId = null)
            => _db.Customers.AsNoTracking()
                .AnyAsync(x => x.PhoneNumber == phone && (ignoreId == null || x.Id != ignoreId));
    }
}
