using SkillForge.Api.Models;

namespace SkillForge.Api.DTOs.Exchange
{
    public class ExchangeDto
    {
        public int Id { get; set; }
        public int OffererId { get; set; }
        public string OffererName { get; set; } = string.Empty;
        public string? OffererProfileImageUrl { get; set; }
        public int LearnerId { get; set; }
        public string LearnerName { get; set; } = string.Empty;
        public string? LearnerProfileImageUrl { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategory { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public double Duration { get; set; }
        public ExchangeStatus Status { get; set; }
        public string? MeetingLink { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool CanReview { get; set; }
        public bool HasReviewed { get; set; }
    }
}