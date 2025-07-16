namespace SkillForge.Api.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
        public int TimeCredits { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}