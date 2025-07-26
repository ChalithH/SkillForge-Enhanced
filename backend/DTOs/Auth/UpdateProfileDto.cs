using System.ComponentModel.DataAnnotations;
using SkillForge.Api.Attributes;

namespace SkillForge.Api.DTOs.Auth
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        [OptionalUrl]
        public string? ProfileImageUrl { get; set; }
    }
}