namespace SkillForge.Api.Services
{
    public interface IUserPresenceService
    {
        Task UserConnectedAsync(int userId, string connectionId);
        Task UserDisconnectedAsync(string connectionId);
        Task<bool> IsUserOnlineAsync(int userId);
        Task<List<int>> GetOnlineUserIdsAsync();
        Task<List<string>> GetUserConnectionIdsAsync(int userId);
        Task<int?> GetUserIdByConnectionIdAsync(string connectionId);
    }
}