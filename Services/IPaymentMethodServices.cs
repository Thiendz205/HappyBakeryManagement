using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface IPaymentMethodServices
    {
        List<PaymentMethodDTO> GetAll();
        PaymentMethod FindById(int id);
        bool Add(PaymentMethod payment);
        bool Update(PaymentMethod payment);
        bool Delete(int id);
        bool HasOrders(int id);
        bool ExistsByName(string name, int? excludeId = null);
    }
}
