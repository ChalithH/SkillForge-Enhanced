namespace SkillForge.Api.Models
{
    public class UserSkill
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public int ProficiencyLevel { get; set; } // 1-5
        public bool IsOffering { get; set; }
        public string? Description { get; set; }
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}