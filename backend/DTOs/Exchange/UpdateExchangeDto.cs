using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.DTOs.Exchange
{
    public class UpdateExchangeDto
    {
        public DateTime? ScheduledAt { get; set; }
        
        [Range(0.5, 4.0)]
        public double? Duration { get; set; }
        
        [MaxLength(500)]
        public string? MeetingLink { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}