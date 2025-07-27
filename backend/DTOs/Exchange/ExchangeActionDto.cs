using System.ComponentModel.DataAnnotations;

namespace SkillForge.Api.DTOs.Exchange
{
    public class ExchangeActionDto
    {
        [MaxLength(2000)]
        public string? Notes { get; set; }
    }
}