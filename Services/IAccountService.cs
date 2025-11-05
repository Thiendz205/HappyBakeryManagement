using HappyBakeryManagement.DTO;

namespace HappyBakeryManagement.Services
{
    public interface IAccountService
    {
        Task<List<AccountDTO>> GetAllAccountsAsync(string roleFilter = null, string searchName = null, int page = 1, int pageSize = 20);
        Task<int> GetTotalCountAsync(string roleFilter = null, string searchName = null);

        Task<(bool isSuccess, string message)> CreateAccountAsync(string email, string password, string role);
        Task LockAccountAsync(string id);
        Task UnlockAccountAsync(string id);
        Task DeleteAccountAsync(string id);
    }
}
