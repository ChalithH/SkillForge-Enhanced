using SkillForge.Api.Models;

namespace SkillForge.Api.Services
{
    public interface INotificationService
    {
        Task SendExchangeRequestNotificationAsync(SkillExchange exchange);
        Task SendExchangeStatusUpdateNotificationAsync(SkillExchange exchange, ExchangeStatus previousStatus);
        Task SendCreditTransferNotificationAsync(int userId, int amount, string reason);
        Task SendGeneralNotificationAsync(int userId, string message, string type = "info");
        Task BroadcastUserPresenceUpdateAsync(int userId, bool isOnline);
        Task SendToGroupAsync(string groupName, string method, object? data);
    }
}