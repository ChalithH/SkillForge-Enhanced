namespace SkillForge.Api.Models
{
    public class SkillExchange
    {
        public int Id { get; set; }
        public int OffererId { get; set; }
        public int LearnerId { get; set; }
        public int SkillId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Duration { get; set; } // in minutes
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Completed, Cancelled
        public string? MeetingLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation properties
        public User Offerer { get; set; } = null!;
        public User Learner { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}