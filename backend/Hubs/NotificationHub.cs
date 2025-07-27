using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SkillForge.Api.Services;
using System.Security.Claims;

namespace SkillForge.Api.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IUserPresenceService _userPresenceService;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(IUserPresenceService userPresenceService, ILogger<NotificationHub> logger)
        {
            _userPresenceService = userPresenceService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                await _userPresenceService.UserConnectedAsync(userId.Value, Context.ConnectionId);
                
                // Join user to their personal group for targeted notifications
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
                
                // Notify other users that this user came online
                await Clients.Others.SendAsync("UserOnline", userId.Value);
                
                _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId.Value, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("Anonymous user attempted to connect to NotificationHub");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = await _userPresenceService.GetUserIdByConnectionIdAsync(Context.ConnectionId);
            
            await _userPresenceService.UserDisconnectedAsync(Context.ConnectionId);

            if (userId.HasValue)
            {
                // Remove from user group
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
                
                // Check if user is still online (has other connections)
                var isStillOnline = await _userPresenceService.IsUserOnlineAsync(userId.Value);
                if (!isStillOnline)
                {
                    // Notify other users that this user went offline
                    await Clients.Others.SendAsync("UserOffline", userId.Value);
                }
                
                _logger.LogInformation("User {UserId} disconnected with connection {ConnectionId}. Still online: {IsOnline}", 
                    userId.Value, Context.ConnectionId, isStillOnline);
            }

            if (exception != null)
            {
                _logger.LogError(exception, "User disconnected with error");
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client can call this method to send a message to another user
        public async Task SendNotificationToUser(int targetUserId, string message, string type = "info")
        {
            var senderId = GetCurrentUserId();
            if (senderId.HasValue)
            {
                await Clients.Group($"User_{targetUserId}").SendAsync("ReceiveNotification", new
                {
                    Message = message,
                    Type = type,
                    SenderId = senderId.Value,
                    Timestamp = DateTime.UtcNow
                });
                
                _logger.LogInformation("User {SenderId} sent notification to User {TargetUserId}: {Message}", 
                    senderId.Value, targetUserId, message);
            }
        }

        // Client can call this method to join a specific group (e.g., for exchange notifications)
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} joined group {GroupName}", Context.ConnectionId, groupName);
        }

        // Client can call this method to leave a specific group
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("Connection {ConnectionId} left group {GroupName}", Context.ConnectionId, groupName);
        }

        // Get current user's online status
        public async Task<bool> GetUserOnlineStatus(int userId)
        {
            return await _userPresenceService.IsUserOnlineAsync(userId);
        }

        // Get list of online users
        public async Task<List<int>> GetOnlineUsers()
        {
            return await _userPresenceService.GetOnlineUserIdsAsync();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}