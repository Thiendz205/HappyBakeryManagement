using HappyBakeryManagement.DTO;
using HappyBakeryManagement.Models;

namespace HappyBakeryManagement.Services
{
    public interface IProductServices
    {
        List<ProductDTO> GetAllProducts();
        Task<bool> ExistsByNameAsync(string name);
        Task AddAsync(ProductDTO productDTO);
        void DeleteProduct(int id);
        ProductDTO GetProductById(int id);
        void UpdateProduct(ProductDTO dto);
        List<ProductDTO> GetAvailableProducts();






    }
}
