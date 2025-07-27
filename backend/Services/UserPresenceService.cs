using System.Collections.Concurrent;

namespace SkillForge.Api.Services
{
    public class UserPresenceService : IUserPresenceService
    {
        // Thread-safe collections for tracking user presence
        private readonly ConcurrentDictionary<int, HashSet<string>> _userConnections = new();
        private readonly ConcurrentDictionary<string, int> _connectionUsers = new();
        private readonly object _lock = new object();

        public Task UserConnectedAsync(int userId, string connectionId)
        {
            lock (_lock)
            {
                // Add connection to user's connection list
                _userConnections.AddOrUpdate(userId, 
                    new HashSet<string> { connectionId },
                    (key, existing) => 
                    {
                        existing.Add(connectionId);
                        return existing;
                    });

                // Map connection to user
                _connectionUsers[connectionId] = userId;
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnectedAsync(string connectionId)
        {
            lock (_lock)
            {
                if (_connectionUsers.TryRemove(connectionId, out int userId))
                {
                    if (_userConnections.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(connectionId);
                        
                        // If no more connections, remove user entirely
                        if (connections.Count == 0)
                        {
                            _userConnections.TryRemove(userId, out _);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task<bool> IsUserOnlineAsync(int userId)
        {
            var isOnline = _userConnections.ContainsKey(userId) && 
                          _userConnections[userId].Count > 0;
            return Task.FromResult(isOnline);
        }

        public Task<List<int>> GetOnlineUserIdsAsync()
        {
            var onlineUsers = _userConnections.Keys.ToList();
            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetUserConnectionIdsAsync(int userId)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                return Task.FromResult(connections.ToList());
            }
            return Task.FromResult(new List<string>());
        }

        public Task<int?> GetUserIdByConnectionIdAsync(string connectionId)
        {
            if (_connectionUsers.TryGetValue(connectionId, out int userId))
            {
                return Task.FromResult<int?>(userId);
            }
            return Task.FromResult<int?>(null);
        }
    }
}