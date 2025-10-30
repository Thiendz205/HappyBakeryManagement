using HappyBakeryManagement.Data;
using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyBakeryManagement.Services
{
    public class OrderDetailsService : IOrderDetailsService
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsService(ApplicationDbContext db)
        {
            _db = db;
        }

        // 🔹 Lấy toàn bộ chi tiết đơn hàng
        public List<OrderDetailsDTO> GetAll()
        {
            return _db.OrderDetails
                .Include(o => o.Order)
                .ThenInclude(c => c.Customer)
                .Include(p => p.Product)
                .Select(x => new OrderDetailsDTO
                {
                    Id = x.Id,
                    OrderID = x.OrderID,
                    ProductID = x.ProductID,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount,
                    ProductName = x.Product.Name,
                    OrderCode = "Order#" + x.OrderID,
                    CustomerName = x.Order.Customer.FullName
                })
                .ToList();
        }
        public List<OrderDetailsDTO> GetFilteredAndSortedPaged(
    string? customerName, string? productName,
    string? sortColumn, string? sortOrder,
    int page, int pageSize)
        {
            var query = _db.OrderDetails
                .Include(o => o.Order).ThenInclude(c => c.Customer)
                .Include(p => p.Product)
                .AsQueryable();

            // 🔍 Tìm kiếm
            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(x => x.Order.Customer.FullName.Contains(customerName));

            if (!string.IsNullOrEmpty(productName))
                query = query.Where(x => x.Product.Name.Contains(productName));

            // 🔽 Sắp xếp
            switch (sortColumn)
            {
                case "CustomerName":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.Order.Customer.FullName)
                        : query.OrderBy(x => x.Order.Customer.FullName);
                    break;

                case "ProductName":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.Product.Name)
                        : query.OrderBy(x => x.Product.Name);
                    break;

                case "Quantity":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.Quantity)
                        : query.OrderBy(x => x.Quantity);
                    break;

                case "TotalAmount":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(x => x.TotalAmount)
                        : query.OrderBy(x => x.TotalAmount);
                    break;

                default:
                    query = query.OrderByDescending(x => x.Id);
                    break;
            }

            // 📄 Phân trang
            return query.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderDetailsDTO
                {
                    Id = x.Id,
                    OrderID = x.OrderID,
                    ProductID = x.ProductID,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount,
                    ProductName = x.Product.Name,
                    OrderCode = "Order#" + x.OrderID,
                    CustomerName = x.Order.Customer.FullName
                })
                .ToList();
        }

        public int GetFilteredCount(string? customerName, string? productName)
        {
            var query = _db.OrderDetails
                .Include(o => o.Order).ThenInclude(c => c.Customer)
                .Include(p => p.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(x => x.Order.Customer.FullName.Contains(customerName));

            if (!string.IsNullOrEmpty(productName))
                query = query.Where(x => x.Product.Name.Contains(productName));

            return query.Count();
        }

        public OrderDetailsDTO? GetById(int id)
        {
            var x = _db.OrderDetails
                .Include(o => o.Order)
                .ThenInclude(c => c.Customer)
                .Include(p => p.Product)
                .FirstOrDefault(x => x.Id == id);
            if (x == null) return null;

            return new OrderDetailsDTO
            {
                Id = x.Id,
                OrderID = x.OrderID,
                ProductID = x.ProductID,
                Quantity = x.Quantity,
                TotalAmount = x.TotalAmount,
                ProductName = x.Product.Name,
                OrderCode = "Order#" + x.OrderID,
                CustomerName = x.Order.Customer.FullName
            };
        }
        public bool Add(OrderDetailsDTO dto)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == dto.ProductID);
            if (product == null) return false;

            decimal total = product.Price * dto.Quantity;

            var entity = new OrderDetails
            {
                OrderID = dto.OrderID,
                ProductID = dto.ProductID,
                Quantity = dto.Quantity,
                TotalAmount = total
            };

            _db.OrderDetails.Add(entity);
            _db.SaveChanges();
            return true;
        }


        // 🔹 Cập nhật
        public bool Update(OrderDetailsDTO dto)
        {
            var entity = _db.OrderDetails.FirstOrDefault(x => x.Id == dto.Id);
            if (entity == null) return false;

            var product = _db.Products.FirstOrDefault(p => p.Id == dto.ProductID);
            if (product == null) return false;

            entity.OrderID = dto.OrderID;
            entity.ProductID = dto.ProductID;
            entity.Quantity = dto.Quantity;
            entity.TotalAmount = product.Price * dto.Quantity;

            _db.SaveChanges();
            return true;
        }

        // 🔹 Xóa
        public bool Delete(int id)
        {
            var entity = _db.OrderDetails.FirstOrDefault(x => x.Id == id);
            if (entity == null) return false;

            _db.OrderDetails.Remove(entity);
            _db.SaveChanges();
            return true;
        }
        public List<Product> GetAllProducts()
        {
            return _db.Products
                .OrderBy(p => p.Name)
                .ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _db.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.Id)
                .ToList();
        }
        public OrderDetailsDTO? FindById(int id)
        {
            var o = _db.OrderDetails
                .Include(x => x.Order)
                .ThenInclude(o => o.Customer)  
                .Include(x => x.Product)
                .FirstOrDefault(x => x.Id == id);

            if (o == null) return null;

            return new OrderDetailsDTO
            {
                Id = o.Id,
                OrderID = o.OrderID,
                ProductID = o.ProductID,
                Quantity = o.Quantity,
                TotalAmount = o.TotalAmount,
                ProductName = o.Product?.Name,
                CustomerName = o.Order?.Customer?.FullName 
            };
        }
        public List<OrderDetailsDTO> GetPaged(int page, int pageSize)
        {
            return _db.OrderDetails
                .Include(o => o.Order)
                .ThenInclude(c => c.Customer)
                .Include(p => p.Product)
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new OrderDetailsDTO
                {
                    Id = x.Id,
                    OrderID = x.OrderID,
                    ProductID = x.ProductID,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount,
                    ProductName = x.Product.Name,
                    OrderCode = "Order#" + x.OrderID,
                    CustomerName = x.Order.Customer.FullName
                })
                .ToList();
        }

        public int GetTotalCount()
        {
            return _db.OrderDetails.Count();
        }

    }
}
