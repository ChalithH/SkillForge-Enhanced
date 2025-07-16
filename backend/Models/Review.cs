namespace SkillForge.Api.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ExchangeId { get; set; }
        public int ReviewerId { get; set; }
        public int ReviewedUserId { get; set; }
        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public SkillExchange Exchange { get; set; } = null!;
        public User Reviewer { get; set; } = null!;
        public User ReviewedUser { get; set; } = null!;
    }
}