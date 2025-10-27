using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface IProductServices
    {
        List<ProductDTO> GetAllProducts();
    }
}
