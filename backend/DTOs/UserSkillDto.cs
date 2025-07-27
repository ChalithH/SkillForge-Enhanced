namespace SkillForge.Api.DTOs
{
    public class UserSkillDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public int ProficiencyLevel { get; set; }
        public bool IsOffering { get; set; }
        public string? Description { get; set; }
        public SkillDto? Skill { get; set; }
    }

    public class SkillDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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