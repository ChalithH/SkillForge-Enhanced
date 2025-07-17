namespace SkillForge.Api.DTOs
{
    public class UserSkillDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategory { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; }
        public bool IsOffering { get; set; }
        public string? Description { get; set; }
    }

    public class CreateUserSkillDto
    {
        public int SkillId { get; set; }
        public int ProficiencyLevel { get; set; }
        public bool IsOffering { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateUserSkillDto
    {
        public int ProficiencyLevel { get; set; }
        public bool IsOffering { get; set; }
        public string? Description { get; set; }
    }
}