using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.Api.Data;
using SkillForge.Api.Models;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/skills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            return await _context.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // GET: api/skills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
        }

        // GET: api/skills/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.Skills
                .Select(s => s.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categories;
        }

        // GET: api/skills/category/{category}
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsByCategory(string category)
        {
            var skills = await _context.Skills
                .Where(s => s.Category.ToLower() == category.ToLower())
                .OrderBy(s => s.Name)
                .ToListAsync();

            if (!skills.Any())
            {
                return NotFound($"No skills found in category '{category}'");
            }

            return skills;
        }

    }
}