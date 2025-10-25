using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly ApplicationDbContext _db;

        public OrderServices(ApplicationDbContext db)
        {
            _db = db;
        }

        // 🔹 Lấy tất cả order, chuyển sang DTO để hiển thị tên Customer và PaymentMethod
        public List<OrderDTO> GetAllOrders()
        {
            var query = from o in _db.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.PaymentMethod)
                        select new OrderDTO
                        {
                            Id = o.Id,
                            BookingDate = o.BookingDate,
                            Status = o.Status,
                            DeliveryAddress = o.DeliveryAddress,
                            PhoneNumber = o.PhoneNumber,
                            Note = o.Note,
                            PaymentMethodID = o.PaymentMethodID,
                            PaymentMethodName = o.PaymentMethod.NamePaymentMethod,
                            CustomerID = o.CustomerID,
                            CustomerName = o.Customer.FullName
                        };
            return query.ToList();
        }

        // 🔹 Lấy tất cả khách hàng (đổ combobox)
        public List<Customer> GetAllCustomers()
        {
            return _db.Customers.ToList();
        }

        // 🔹 Lấy tất cả phương thức thanh toán (đổ combobox)
        public List<PaymentMethod> GetAllPaymentMethods()
        {
            return _db.PaymentMethods.ToList();
        }

        // 🔹 Thêm đơn hàng
        public bool AddOrder(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return true;
        }

        // 🔹 Tìm đơn theo ID (để Edit hoặc Delete)
        public Order FindOrderById(int id)
        {
            return _db.Orders.FirstOrDefault(o => o.Id == id);
        }

        // 🔹 Cập nhật đơn
        public bool UpdateOrder(Order order)
        {
            var entity = _db.Orders.FirstOrDefault(o => o.Id == order.Id);
            if (entity != null)
            {
                entity.BookingDate = order.BookingDate;
                entity.Status = order.Status;
                entity.DeliveryAddress = order.DeliveryAddress;
                entity.PhoneNumber = order.PhoneNumber;
                entity.Note = order.Note;
                entity.PaymentMethodID = order.PaymentMethodID;
                entity.CustomerID = order.CustomerID;

                _db.SaveChanges();
                return true;
            }
            return false;
        }
        // 🔹 Lấy danh sách theo trang (phân trang)
        public List<OrderDTO> GetOrdersPaged(int page, int pageSize)
        {
            return GetAllOrders()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        // 🔹 Đếm tổng số đơn
        public int GetTotalOrders()
        {
            return _db.Orders.Count();
        }

    }
}
