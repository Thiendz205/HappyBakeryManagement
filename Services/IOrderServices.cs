using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface IOrderServices
    {
        List<OrderDTO> GetAllOrders();
        Order FindOrderById(int id);
        bool AddOrder(Order order);
        bool UpdateOrder(Order order);
        List<Customer> GetAllCustomers();
        List<PaymentMethod> GetAllPaymentMethods();
        public List<OrderDTO> GetOrdersPaged(int page, int pageSize, string sortColumn = "BookingDate", string sortOrder = "asc");
        int GetTotalOrders();
        List<OrderDTO> SearchOrders(string customerName, string phoneNumber, string status, string paymentMethodName, int page, int pageSize);
        int GetTotalSearchedOrders(string customerName, string phoneNumber, string status, string paymentMethodName);
        public bool PlaceOrder(int customerId, string address, string phone, string? note, int paymentMethodId);
    }
}
