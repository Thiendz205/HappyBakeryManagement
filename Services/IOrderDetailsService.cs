using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface IOrderDetailsService
    {
        List<OrderDetailsDTO> GetAll();
        OrderDetailsDTO? GetById(int id);
        bool Add(OrderDetailsDTO dto);
        bool Update(OrderDetailsDTO dto);
        bool Delete(int id);
        public List<Product> GetAllProducts();
        public List<Order> GetAllOrders();
        public OrderDetailsDTO? FindById(int id);
        public List<OrderDetailsDTO> GetPaged(int page, int pageSize);
        public int GetTotalCount();
        public List<OrderDetailsDTO> GetFilteredAndSortedPaged(
   string? customerName, string? productName,
   string? sortColumn, string? sortOrder,
   int page, int pageSize);
        public int GetFilteredCount(string? customerName, string? productName);
    }
}
