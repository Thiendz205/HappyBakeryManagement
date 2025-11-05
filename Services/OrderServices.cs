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
        public List<OrderDTO> SearchOrders(string customerName, string phoneNumber, string status, string paymentMethodName, int page, int pageSize)
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

            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(o => o.CustomerName.Contains(customerName));

            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(o => o.PhoneNumber.Contains(phoneNumber));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.Contains(status));

            if (!string.IsNullOrEmpty(paymentMethodName))
                query = query.Where(o => o.PaymentMethodName.Contains(paymentMethodName));

            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public int GetTotalSearchedOrders(string customerName, string phoneNumber, string status, string paymentMethodName)
        {
            var query = _db.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(o => o.Customer.FullName.Contains(customerName));

            if (!string.IsNullOrEmpty(phoneNumber))
                query = query.Where(o => o.PhoneNumber.Contains(phoneNumber));

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.Contains(status));

            if (!string.IsNullOrEmpty(paymentMethodName))
                query = query.Where(o => o.PaymentMethod.NamePaymentMethod.Contains(paymentMethodName));

            return query.Count();
        }


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
        public List<OrderDTO> GetOrdersPaged(int page, int pageSize, string sortColumn = "BookingDate", string sortOrder = "asc")
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

            // ✅ Sắp xếp theo cột được chọn
            switch (sortColumn)
            {
                case "CustomerName":
                    query = (sortOrder == "asc") ? query.OrderBy(o => o.CustomerName) : query.OrderByDescending(o => o.CustomerName);
                    break;
                case "Status":
                    query = (sortOrder == "asc") ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status);
                    break;
                case "PaymentMethodName":
                    query = (sortOrder == "asc") ? query.OrderBy(o => o.PaymentMethodName) : query.OrderByDescending(o => o.PaymentMethodName);
                    break;
                default:
                    query = (sortOrder == "asc") ? query.OrderBy(o => o.BookingDate) : query.OrderByDescending(o => o.BookingDate);
                    break;
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }


        // 🔹 Đếm tổng số đơn
        public int GetTotalOrders()
        {
            return _db.Orders.Count();
        }
        public bool PlaceOrder(int customerId, string address, string phone, string? note, int paymentMethodId)
        {
            // Lấy danh sách sản phẩm trong giỏ hàng của khách
            var cartItems = _db.Carts
                .Include(c => c.Product)
                .Where(c => c.CustomerID == customerId)
                .ToList();

            if (cartItems == null || cartItems.Count == 0)
                throw new InvalidOperationException("Giỏ hàng của bạn đang trống.");

            // Tạo đơn hàng mới
            var order = new Order
            {
                BookingDate = DateTime.Now,
                CustomerID = customerId,
                DeliveryAddress = address,
                PhoneNumber = phone,
                Note = note,
                PaymentMethodID = paymentMethodId,
                Status = "Chờ xác nhận"
            };

            _db.Orders.Add(order);
            _db.SaveChanges(); // cần Save để có OrderID

            // Thêm chi tiết đơn hàng
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetails
                {
                    OrderID = order.Id,
                    ProductID = item.ProductId,
                    Quantity = item.Quantity,
                    TotalAmount = item.Quantity * item.Product.Price
                };
                _db.OrderDetails.Add(orderDetail);
            }

            // Xóa giỏ hàng
            _db.Carts.RemoveRange(cartItems);

            // Lưu thay đổi
            _db.SaveChanges();

            return true;
        }
    }
}
