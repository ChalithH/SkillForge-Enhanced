using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.Api.Data;
using SkillForge.Api.DTOs;
using SkillForge.Api.Models;
using System.Security.Claims;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserSkillsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserSkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/userskills
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSkillDto>>> GetMySkills()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var userSkills = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.UserId == userId)
                .Select(us => new UserSkillDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SkillId = us.SkillId,
                    SkillName = us.Skill.Name,
                    SkillCategory = us.Skill.Category,
                    ProficiencyLevel = us.ProficiencyLevel,
                    IsOffering = us.IsOffering,
                    Description = us.Description
                })
                .ToListAsync();

            return userSkills;
        }

        // GET: api/userskills/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserSkillDto>> GetUserSkill(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var userSkill = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.Id == id && us.UserId == userId)
                .Select(us => new UserSkillDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SkillId = us.SkillId,
                    SkillName = us.Skill.Name,
                    SkillCategory = us.Skill.Category,
                    ProficiencyLevel = us.ProficiencyLevel,
                    IsOffering = us.IsOffering,
                    Description = us.Description
                })
                .FirstOrDefaultAsync();

            if (userSkill == null)
            {
                return NotFound();
            }

            return userSkill;
        }

        // POST: api/userskills
        [HttpPost]
        public async Task<ActionResult<UserSkillDto>> CreateUserSkill(CreateUserSkillDto createDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if skill exists
            var skillExists = await _context.Skills.AnyAsync(s => s.Id == createDto.SkillId);
            if (!skillExists)
            {
                return BadRequest("Invalid skill ID");
            }

            // Check if user already has this skill
            var existingUserSkill = await _context.UserSkills
                .AnyAsync(us => us.UserId == userId && us.SkillId == createDto.SkillId);
            if (existingUserSkill)
            {
                return BadRequest("You already have this skill");
            }

            // Validate proficiency level
            if (createDto.ProficiencyLevel < 1 || createDto.ProficiencyLevel > 5)
            {
                return BadRequest("Proficiency level must be between 1 and 5");
            }

            var userSkill = new UserSkill
            {
                UserId = userId,
                SkillId = createDto.SkillId,
                ProficiencyLevel = createDto.ProficiencyLevel,
                IsOffering = createDto.IsOffering,
                Description = createDto.Description
            };

            _context.UserSkills.Add(userSkill);
            await _context.SaveChangesAsync();

            // Fetch the created skill with related data
            var createdSkill = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.Id == userSkill.Id)
                .Select(us => new UserSkillDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SkillId = us.SkillId,
                    SkillName = us.Skill.Name,
                    SkillCategory = us.Skill.Category,
                    ProficiencyLevel = us.ProficiencyLevel,
                    IsOffering = us.IsOffering,
                    Description = us.Description
                })
                .FirstAsync();

            return CreatedAtAction(nameof(GetUserSkill), new { id = createdSkill.Id }, createdSkill);
        }

        // PUT: api/userskills/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserSkill(int id, UpdateUserSkillDto updateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var userSkill = await _context.UserSkills
                .FirstOrDefaultAsync(us => us.Id == id && us.UserId == userId);

            if (userSkill == null)
            {
                return NotFound();
            }

            // Validate proficiency level
            if (updateDto.ProficiencyLevel < 1 || updateDto.ProficiencyLevel > 5)
            {
                return BadRequest("Proficiency level must be between 1 and 5");
            }

            userSkill.ProficiencyLevel = updateDto.ProficiencyLevel;
            userSkill.IsOffering = updateDto.IsOffering;
            userSkill.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/userskills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserSkill(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var userSkill = await _context.UserSkills
                .FirstOrDefaultAsync(us => us.Id == id && us.UserId == userId);

            if (userSkill == null)
            {
                return NotFound();
            }

            _context.UserSkills.Remove(userSkill);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/userskills/offering
        [HttpGet("offering")]
        public async Task<ActionResult<IEnumerable<UserSkillDto>>> GetMyOfferingSkills()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var offeringSkills = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.UserId == userId && us.IsOffering)
                .Select(us => new UserSkillDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SkillId = us.SkillId,
                    SkillName = us.Skill.Name,
                    SkillCategory = us.Skill.Category,
                    ProficiencyLevel = us.ProficiencyLevel,
                    IsOffering = us.IsOffering,
                    Description = us.Description
                })
                .ToListAsync();

            return offeringSkills;
        }

        // GET: api/userskills/learning
        [HttpGet("learning")]
        public async Task<ActionResult<IEnumerable<UserSkillDto>>> GetMyLearningSkills()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var learningSkills = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.UserId == userId && !us.IsOffering)
                .Select(us => new UserSkillDto
                {
                    Id = us.Id,
                    UserId = us.UserId,
                    SkillId = us.SkillId,
                    SkillName = us.Skill.Name,
                    SkillCategory = us.Skill.Category,
                    ProficiencyLevel = us.ProficiencyLevel,
                    IsOffering = us.IsOffering,
                    Description = us.Description
                })
                .ToListAsync();

            return learningSkills;
        }
    }
}