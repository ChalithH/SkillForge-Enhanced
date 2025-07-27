using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        [Required]
        public int ExchangeId { get; set; }
        public SkillExchange? Exchange { get; set; }
        
        [Required]
        public int ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        
        [Required]
        public int ReviewedUserId { get; set; }
        public User? ReviewedUser { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(1000)]
        public string? Comment { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}