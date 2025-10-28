using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public class PaymentMethodServices: IPaymentMethodServices
    {
        private readonly ApplicationDbContext _db;

        public PaymentMethodServices(ApplicationDbContext db)
        {
            _db = db;
        }

        // 🔹 Lấy tất cả
        public List<PaymentMethodDTO> GetAll()
        {
            return _db.PaymentMethods
                .Select(pm => new PaymentMethodDTO
                {
                    Id = pm.Id,
                    NamePaymentMethod = pm.NamePaymentMethod
                })
                .ToList();
        }

        // 🔹 Tìm theo ID
        public PaymentMethod FindById(int id)
        {
            return _db.PaymentMethods.FirstOrDefault(p => p.Id == id);
        }

        // 🔹 Thêm mới
        public bool Add(PaymentMethod payment)
        {
            _db.PaymentMethods.Add(payment);
            _db.SaveChanges();
            return true;
        }

        // 🔹 Cập nhật
        public bool Update(PaymentMethod payment)
        {
            var entity = _db.PaymentMethods.FirstOrDefault(p => p.Id == payment.Id);
            if (entity != null)
            {
                entity.NamePaymentMethod = payment.NamePaymentMethod;
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        // 🔹 Kiểm tra có Order nào đang dùng không
        public bool HasOrders(int id)
        {
            return _db.Orders.Any(o => o.PaymentMethodID == id);
        }

        // 🔹 Xóa (nếu chưa có Order nào dùng)
        public bool Delete(int id)
        {
            if (HasOrders(id)) return false;

            var entity = _db.PaymentMethods.FirstOrDefault(p => p.Id == id);
            if (entity != null)
            {
                _db.PaymentMethods.Remove(entity);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        // 🔹 Kiểm tra trùng tên
        public bool ExistsByName(string name, int? excludeId = null)
        {
            name = name.Trim().ToLower();
            return _db.PaymentMethods
                      .Any(p => p.NamePaymentMethod.ToLower() == name
                             && (excludeId == null || p.Id != excludeId));
        }

    }
}
