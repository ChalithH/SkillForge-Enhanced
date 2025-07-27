using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.DTOs.Exchange
{
    public class CreateExchangeDto
    {
        [Required]
        public int OffererId { get; set; }
        
        [Required]
        public int SkillId { get; set; }
        
        [Required]
        public DateTime ScheduledAt { get; set; }
        
        [Required]
        [Range(0.5, 4.0)]
        public double Duration { get; set; }
        
        [MaxLength(500)]
        public string? MeetingLink { get; set; }
        
        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}