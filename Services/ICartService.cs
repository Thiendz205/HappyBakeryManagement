using HappyBakeryManagement.DTO;

namespace HappyBakeryManagement.Services
{
    public interface ICartService
    {
        Task<bool> AddToCartAsync(int customerId, int productId, int quantity = 1);
        List<CartDTO> GetCartByCustomerId(int customerId);
        Task<bool> UpdateQuantityAsync(int cartId, int quantity);
        Task<bool> RemoveFromCartAsync(int cartId);
        Task<bool> ClearCartAsync(int customerId);
        decimal GetCartTotal(int customerId);
        int GetCartItemCount(int customerId);
    }
}

