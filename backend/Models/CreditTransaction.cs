using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.Models
{
    public class CreditTransaction
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }
        
        [Required]
        public int Amount { get; set; } // Positive for credit, negative for debit
        
        [Required]
        public int BalanceAfter { get; set; } // User's balance after this transaction
        
        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty; // "ExchangeComplete", "ExchangeCancel", "AdminAdjustment", "SignupBonus"
        
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        public int? RelatedUserId { get; set; } // For transfers
        public User? RelatedUser { get; set; }
        
        public int? ExchangeId { get; set; } // For exchange-related transactions
        public SkillExchange? Exchange { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}