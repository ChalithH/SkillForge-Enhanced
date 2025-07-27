using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillForge.Api.Data;
using SkillForge.Api.Models;
using SkillForge.Api.DTOs;
using System.Security.Claims;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MatchingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MatchingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("browse")]
        public async Task<ActionResult<PagedResult<UserMatchDto>>> BrowseUsers(
            [FromQuery] string? category = null,
            [FromQuery] double? minRating = null,
            [FromQuery] bool? isOnline = null,
            [FromQuery] string? skillName = null,
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (limit > 50) limit = 50; // Cap the limit
            var offset = (page - 1) * limit;

            var query = _context.Users
                .Where(u => u.Id != currentUserId)
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.ReviewsReceived)
                .AsQueryable();

            // Filter by skill category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(u => u.UserSkills.Any(us => 
                    us.IsOffering && us.Skill!.Category.ToLower() == category.ToLower()));
            }

            // Filter by specific skill name
            if (!string.IsNullOrEmpty(skillName))
            {
                query = query.Where(u => u.UserSkills.Any(us => 
                    us.IsOffering && us.Skill!.Name.ToLower().Contains(skillName.ToLower())));
            }

            var users = await query
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            var userMatches = users.Select(u => new UserMatchDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Bio = u.Bio,
                ProfileImageUrl = u.ProfileImageUrl,
                AverageRating = u.ReviewsReceived.Any() 
                    ? Math.Round(u.ReviewsReceived.Average(r => r.Rating), 1) 
                    : 0,
                ReviewCount = u.ReviewsReceived.Count,
                SkillsOffered = u.UserSkills
                    .Where(us => us.IsOffering)
                    .Select(us => new MatchUserSkillDto
                    {
                        Id = us.Id,
                        SkillId = us.SkillId,
                        SkillName = us.Skill!.Name,
                        SkillCategory = us.Skill.Category,
                        ProficiencyLevel = us.ProficiencyLevel,
                        Description = us.Description
                    }).ToList(),
                CompatibilityScore = CalculateCompatibilityScore(currentUserId, u.Id),
                IsOnline = false // TODO: Implement with SignalR in afternoon tasks
            }).ToList();

            // Apply rating filter after DTO mapping
            if (minRating.HasValue)
            {
                userMatches = userMatches.Where(u => u.AverageRating >= minRating.Value).ToList();
            }

            // Apply online status filter (placeholder for now)
            if (isOnline.HasValue)
            {
                // TODO: Implement with SignalR online status tracking
                userMatches = userMatches.Where(u => u.IsOnline == isOnline.Value).ToList();
            }

            // Sort by compatibility score, then by rating
            userMatches = userMatches
                .OrderByDescending(u => u.CompatibilityScore)
                .ThenByDescending(u => u.AverageRating)
                .ToList();

            var totalCount = await _context.Users.CountAsync(u => u.Id != currentUserId);

            return Ok(new PagedResult<UserMatchDto>
            {
                Items = userMatches,
                TotalCount = totalCount,
                Page = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        [HttpGet("recommendations")]
        public async Task<ActionResult<List<UserMatchDto>>> GetRecommendations([FromQuery] int limit = 10)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (limit > 20) limit = 20; // Cap the limit

            // Get current user's wanted skills
            var currentUserWantedSkills = await _context.UserSkills
                .Where(us => us.UserId == currentUserId && !us.IsOffering)
                .Select(us => us.SkillId)
                .ToListAsync();

            if (!currentUserWantedSkills.Any())
            {
                // If user has no wanted skills, return users with highest ratings
                var topRatedUsers = await _context.Users
                    .Where(u => u.Id != currentUserId)
                    .Include(u => u.UserSkills.Where(us => us.IsOffering))
                        .ThenInclude(us => us.Skill)
                    .Include(u => u.ReviewsReceived)
                    .Where(u => u.UserSkills.Any(us => us.IsOffering))
                    .Take(limit)
                    .ToListAsync();

                return Ok(topRatedUsers.Select(u => CreateUserMatchDto(u, currentUserId)).ToList());
            }

            // Find users who offer skills that the current user wants
            var recommendedUsers = await _context.Users
                .Where(u => u.Id != currentUserId)
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.ReviewsReceived)
                .Where(u => u.UserSkills.Any(us => 
                    us.IsOffering && currentUserWantedSkills.Contains(us.SkillId)))
                .ToListAsync();

            var userMatches = recommendedUsers
                .Select(u => CreateUserMatchDto(u, currentUserId))
                .OrderByDescending(u => u.CompatibilityScore)
                .ThenByDescending(u => u.AverageRating)
                .Take(limit)
                .ToList();

            return Ok(userMatches);
        }

        [HttpGet("compatibility/{targetUserId}")]
        public async Task<ActionResult<CompatibilityAnalysisDto>> GetCompatibilityAnalysis(int targetUserId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var targetUser = await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.ReviewsReceived)
                .FirstOrDefaultAsync(u => u.Id == targetUserId);

            if (targetUser == null)
            {
                return NotFound("User not found");
            }

            var currentUserSkills = await _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.UserId == currentUserId)
                .ToListAsync();

            var analysis = new CompatibilityAnalysisDto
            {
                TargetUserId = targetUserId,
                TargetUserName = targetUser.Name,
                OverallScore = CalculateCompatibilityScore(currentUserId, targetUserId),
                TargetUserRating = targetUser.ReviewsReceived.Any() 
                    ? Math.Round(targetUser.ReviewsReceived.Average(r => r.Rating), 1) 
                    : 0,
                SharedSkills = GetSharedSkills(currentUserSkills, targetUser.UserSkills.ToList()),
                ComplementarySkills = GetComplementarySkills(currentUserSkills, targetUser.UserSkills.ToList()),
                RecommendationReason = GenerateRecommendationReason(currentUserSkills, targetUser.UserSkills.ToList())
            };

            return Ok(analysis);
        }

        private double CalculateCompatibilityScore(int currentUserId, int targetUserId)
        {
            var currentUserSkills = _context.UserSkills
                .Where(us => us.UserId == currentUserId)
                .Include(us => us.Skill)
                .ToList();

            var targetUserSkills = _context.UserSkills
                .Where(us => us.UserId == targetUserId)
                .Include(us => us.Skill)
                .ToList();

            if (!currentUserSkills.Any() || !targetUserSkills.Any())
                return 0;

            var score = 0.0;

            // Check skill compatibility (what I want vs what they offer)
            var myWantedSkills = currentUserSkills.Where(us => !us.IsOffering).ToList();
            var theirOfferedSkills = targetUserSkills.Where(us => us.IsOffering).ToList();

            foreach (var wantedSkill in myWantedSkills)
            {
                var matchingOfferedSkill = theirOfferedSkills
                    .FirstOrDefault(os => os.SkillId == wantedSkill.SkillId);
                
                if (matchingOfferedSkill != null)
                {
                    // Perfect skill match gets high score, weighted by their proficiency
                    score += 30 * (matchingOfferedSkill.ProficiencyLevel / 5.0);
                }
            }

            // Check reverse compatibility (what they want vs what I offer)
            var theirWantedSkills = targetUserSkills.Where(us => !us.IsOffering).ToList();
            var myOfferedSkills = currentUserSkills.Where(us => us.IsOffering).ToList();

            foreach (var theirWantedSkill in theirWantedSkills)
            {
                var myMatchingOfferedSkill = myOfferedSkills
                    .FirstOrDefault(os => os.SkillId == theirWantedSkill.SkillId);
                
                if (myMatchingOfferedSkill != null)
                {
                    // Mutual benefit increases score
                    score += 20 * (myMatchingOfferedSkill.ProficiencyLevel / 5.0);
                }
            }

            // Category overlap bonus
            var myCategories = currentUserSkills.Select(us => us.Skill!.Category).Distinct().ToList();
            var theirCategories = targetUserSkills.Select(us => us.Skill!.Category).Distinct().ToList();
            var sharedCategories = myCategories.Intersect(theirCategories).Count();
            score += sharedCategories * 5;

            return Math.Min(score, 100); // Cap at 100
        }

        private UserMatchDto CreateUserMatchDto(User user, int currentUserId)
        {
            return new UserMatchDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Bio = user.Bio,
                ProfileImageUrl = user.ProfileImageUrl,
                AverageRating = user.ReviewsReceived.Any() 
                    ? Math.Round(user.ReviewsReceived.Average(r => r.Rating), 1) 
                    : 0,
                ReviewCount = user.ReviewsReceived.Count,
                SkillsOffered = user.UserSkills
                    .Where(us => us.IsOffering)
                    .Select(us => new MatchUserSkillDto
                    {
                        Id = us.Id,
                        SkillId = us.SkillId,
                        SkillName = us.Skill!.Name,
                        SkillCategory = us.Skill.Category,
                        ProficiencyLevel = us.ProficiencyLevel,
                        Description = us.Description
                    }).ToList(),
                CompatibilityScore = CalculateCompatibilityScore(currentUserId, user.Id),
                IsOnline = false // TODO: Implement with SignalR
            };
        }

        private List<SkillMatchDto> GetSharedSkills(List<UserSkill> currentUserSkills, List<UserSkill> targetUserSkills)
        {
            var sharedSkills = new List<SkillMatchDto>();

            foreach (var mySkill in currentUserSkills)
            {
                var theirMatchingSkill = targetUserSkills
                    .FirstOrDefault(ts => ts.SkillId == mySkill.SkillId);

                if (theirMatchingSkill != null)
                {
                    sharedSkills.Add(new SkillMatchDto
                    {
                        SkillId = mySkill.SkillId,
                        SkillName = mySkill.Skill!.Name,
                        SkillCategory = mySkill.Skill.Category,
                        MyProficiency = mySkill.ProficiencyLevel,
                        TheirProficiency = theirMatchingSkill.ProficiencyLevel,
                        MyRole = mySkill.IsOffering ? "Offering" : "Wanting",
                        TheirRole = theirMatchingSkill.IsOffering ? "Offering" : "Wanting"
                    });
                }
            }

            return sharedSkills;
        }

        private List<SkillMatchDto> GetComplementarySkills(List<UserSkill> currentUserSkills, List<UserSkill> targetUserSkills)
        {
            var complementarySkills = new List<SkillMatchDto>();

            // Skills I want that they offer
            var myWantedSkills = currentUserSkills.Where(us => !us.IsOffering);
            foreach (var wantedSkill in myWantedSkills)
            {
                var theirOfferedSkill = targetUserSkills
                    .FirstOrDefault(ts => ts.SkillId == wantedSkill.SkillId && ts.IsOffering);

                if (theirOfferedSkill != null)
                {
                    complementarySkills.Add(new SkillMatchDto
                    {
                        SkillId = wantedSkill.SkillId,
                        SkillName = wantedSkill.Skill!.Name,
                        SkillCategory = wantedSkill.Skill.Category,
                        MyProficiency = wantedSkill.ProficiencyLevel,
                        TheirProficiency = theirOfferedSkill.ProficiencyLevel,
                        MyRole = "Wanting",
                        TheirRole = "Offering"
                    });
                }
            }

            // Skills they want that I offer
            var theirWantedSkills = targetUserSkills.Where(us => !us.IsOffering);
            foreach (var theirWantedSkill in theirWantedSkills)
            {
                var myOfferedSkill = currentUserSkills
                    .FirstOrDefault(ms => ms.SkillId == theirWantedSkill.SkillId && ms.IsOffering);

                if (myOfferedSkill != null && !complementarySkills.Any(cs => cs.SkillId == theirWantedSkill.SkillId))
                {
                    complementarySkills.Add(new SkillMatchDto
                    {
                        SkillId = theirWantedSkill.SkillId,
                        SkillName = theirWantedSkill.Skill!.Name,
                        SkillCategory = theirWantedSkill.Skill.Category,
                        MyProficiency = myOfferedSkill.ProficiencyLevel,
                        TheirProficiency = theirWantedSkill.ProficiencyLevel,
                        MyRole = "Offering",
                        TheirRole = "Wanting"
                    });
                }
            }

            return complementarySkills;
        }

        private string GenerateRecommendationReason(List<UserSkill> currentUserSkills, List<UserSkill> targetUserSkills)
        {
            var complementarySkills = GetComplementarySkills(currentUserSkills, targetUserSkills);
            
            if (!complementarySkills.Any())
            {
                return "This user has skills in similar categories and could provide good learning opportunities.";
            }

            var skillsTheyOffer = complementarySkills.Where(cs => cs.TheirRole == "Offering").Take(2);
            var skillsTheyWant = complementarySkills.Where(cs => cs.TheirRole == "Wanting").Take(2);

            var reasons = new List<string>();

            if (skillsTheyOffer.Any())
            {
                var skillNames = string.Join(", ", skillsTheyOffer.Select(s => s.SkillName));
                reasons.Add($"They can teach you: {skillNames}");
            }

            if (skillsTheyWant.Any())
            {
                var skillNames = string.Join(", ", skillsTheyWant.Select(s => s.SkillName));
                reasons.Add($"You can teach them: {skillNames}");
            }

            return string.Join(". ", reasons) + ".";
        }
    }

    // DTOs for the matching system
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
    }

    public class UserMatchDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<MatchUserSkillDto> SkillsOffered { get; set; } = new();
        public double CompatibilityScore { get; set; }
        public bool IsOnline { get; set; }
    }

    public class MatchUserSkillDto
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategory { get; set; } = string.Empty;
        public int ProficiencyLevel { get; set; }
        public string? Description { get; set; }
    }

    public class CompatibilityAnalysisDto
    {
        public int TargetUserId { get; set; }
        public string TargetUserName { get; set; } = string.Empty;
        public double OverallScore { get; set; }
        public double TargetUserRating { get; set; }
        public List<SkillMatchDto> SharedSkills { get; set; } = new();
        public List<SkillMatchDto> ComplementarySkills { get; set; } = new();
        public string RecommendationReason { get; set; } = string.Empty;
    }

    public class SkillMatchDto
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string SkillCategory { get; set; } = string.Empty;
        public int MyProficiency { get; set; }
        public int TheirProficiency { get; set; }
        public string MyRole { get; set; } = string.Empty; // "Offering" or "Wanting"
        public string TheirRole { get; set; } = string.Empty; // "Offering" or "Wanting"
    }
}