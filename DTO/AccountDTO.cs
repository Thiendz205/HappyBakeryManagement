namespace HappyBakeryManagement.DTO
{
    public class AccountDTO
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool IsLocked { get; set; }
    }
}
