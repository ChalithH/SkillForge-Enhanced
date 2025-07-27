using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.Models
{
    public class SkillExchange
    {
        public int Id { get; set; }
        
        [Required]
        public int OffererId { get; set; }
        public User? Offerer { get; set; }
        
        [Required]
        public int LearnerId { get; set; }
        public User? Learner { get; set; }
        
        [Required]
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
        
        [Required]
        public DateTime ScheduledAt { get; set; }
        
        [Required]
        [Range(0.5, 4.0)] // Minimum 30 minutes, maximum 4 hours
        public double Duration { get; set; } // Duration in hours
        
        [Required]
        public ExchangeStatus Status { get; set; }
        
        [MaxLength(500)]
        public string? MeetingLink { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property for reviews
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
    
    public enum ExchangeStatus
    {
        Pending,      // Initial request state
        Accepted,     // Offerer accepted the request
        Rejected,     // Offerer rejected the request
        Cancelled,    // Either party cancelled
        Completed,    // Exchange finished successfully
        NoShow        // One party didn't attend
    }
}