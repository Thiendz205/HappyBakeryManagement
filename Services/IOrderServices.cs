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
        List<OrderDTO> GetOrdersPaged(int page, int pageSize);
        int GetTotalOrders();
    }
}
