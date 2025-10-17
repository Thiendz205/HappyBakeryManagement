using HappyBakeryManagement.Models;
namespace HappyBakeryManagement.Services
{
    public class CustomerService : ICustomerService
    {
        public readonly HappyBakeryContext _db;
        public CustomerService(HappyBakeryContext db)
        {
            _db = db;
        }
    }
}
