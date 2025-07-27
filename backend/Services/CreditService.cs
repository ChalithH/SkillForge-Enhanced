using Microsoft.EntityFrameworkCore;
using SkillForge.Api.Data;
using SkillForge.Api.Models;
using System.Data;

namespace SkillForge.Api.Services
{
    public interface ICreditService
    {
        Task<bool> TransferCreditsAsync(int fromUserId, int toUserId, int amount, string reason, int? exchangeId = null);
        Task<bool> AddCreditsAsync(int userId, int amount, string reason);
        Task<bool> DeductCreditsAsync(int userId, int amount, string reason);
        Task<int> GetUserCreditsAsync(int userId);
        Task<IEnumerable<CreditTransaction>> GetUserCreditHistoryAsync(int userId, int limit = 50);
    }

    public class CreditService : ICreditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreditService> _logger;

        public CreditService(ApplicationDbContext context, ILogger<CreditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> TransferCreditsAsync(int fromUserId, int toUserId, int amount, string reason, int? exchangeId = null)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive", nameof(amount));
            }

            if (fromUserId == toUserId)
            {
                throw new ArgumentException("Cannot transfer credits to yourself");
            }

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Get both users with row-level locking
                var fromUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == fromUserId);
                var toUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == toUserId);

                if (fromUser == null || toUser == null)
                {
                    throw new InvalidOperationException("One or both users not found");
                }

                // Check sufficient balance
                if (fromUser.TimeCredits < amount)
                {
                    throw new InvalidOperationException($"Insufficient credits. User has {fromUser.TimeCredits} but needs {amount}");
                }

                // Perform the transfer
                fromUser.TimeCredits -= amount;
                toUser.TimeCredits += amount;
                fromUser.UpdatedAt = DateTime.UtcNow;
                toUser.UpdatedAt = DateTime.UtcNow;

                // Record both sides of the transaction
                var fromTransaction = new CreditTransaction
                {
                    UserId = fromUserId,
                    Amount = -amount,
                    BalanceAfter = fromUser.TimeCredits,
                    TransactionType = "ExchangeComplete",
                    Reason = reason,
                    RelatedUserId = toUserId,
                    ExchangeId = exchangeId,
                    CreatedAt = DateTime.UtcNow
                };

                var toTransaction = new CreditTransaction
                {
                    UserId = toUserId,
                    Amount = amount,
                    BalanceAfter = toUser.TimeCredits,
                    TransactionType = "ExchangeComplete",
                    Reason = reason,
                    RelatedUserId = fromUserId,
                    ExchangeId = exchangeId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CreditTransactions.Add(fromTransaction);
                _context.CreditTransactions.Add(toTransaction);

                _logger.LogInformation($"Credit transfer: {amount} credits from user {fromUserId} to user {toUserId}. Reason: {reason}. ExchangeId: {exchangeId}");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Failed to transfer {amount} credits from user {fromUserId} to user {toUserId}");
                throw;
            }
        }

        public async Task<bool> AddCreditsAsync(int userId, int amount, string reason)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive", nameof(amount));
            }

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                user.TimeCredits += amount;
                user.UpdatedAt = DateTime.UtcNow;

                // Record the transaction
                var creditTransaction = new CreditTransaction
                {
                    UserId = userId,
                    Amount = amount,
                    BalanceAfter = user.TimeCredits,
                    TransactionType = "AdminAdjustment",
                    Reason = reason,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CreditTransactions.Add(creditTransaction);

                _logger.LogInformation($"Added {amount} credits to user {userId}. Reason: {reason}");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Failed to add {amount} credits to user {userId}");
                throw;
            }
        }

        public async Task<bool> DeductCreditsAsync(int userId, int amount, string reason)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be positive", nameof(amount));
            }

            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("User not found");
                }

                if (user.TimeCredits < amount)
                {
                    throw new InvalidOperationException($"Insufficient credits. User has {user.TimeCredits} but needs {amount}");
                }

                user.TimeCredits -= amount;
                user.UpdatedAt = DateTime.UtcNow;

                // Record the transaction
                var creditTransaction = new CreditTransaction
                {
                    UserId = userId,
                    Amount = -amount,
                    BalanceAfter = user.TimeCredits,
                    TransactionType = "AdminAdjustment",
                    Reason = reason,
                    CreatedAt = DateTime.UtcNow
                };

                _context.CreditTransactions.Add(creditTransaction);

                _logger.LogInformation($"Deducted {amount} credits from user {userId}. Reason: {reason}");

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Failed to deduct {amount} credits from user {userId}");
                throw;
            }
        }

        public async Task<int> GetUserCreditsAsync(int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.TimeCredits ?? 0;
        }

        public async Task<IEnumerable<CreditTransaction>> GetUserCreditHistoryAsync(int userId, int limit = 50)
        {
            return await _context.CreditTransactions
                .Include(ct => ct.RelatedUser)
                .Include(ct => ct.Exchange)
                .Where(ct => ct.UserId == userId)
                .OrderByDescending(ct => ct.CreatedAt)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}