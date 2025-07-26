using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillForge.Api.DTOs.Auth;
using SkillForge.Api.Services;
using System.Security.Claims;

namespace SkillForge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                
                if (result == null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                _logger.LogInformation($"New user registered: {result.Email}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid email or password." });
                }

                _logger.LogInformation($"User logged in: {result.Email}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    name = user.Name,
                    timeCredits = user.TimeCredits,
                    bio = user.Bio,
                    profileImageUrl = user.ProfileImageUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, new { message = "An error occurred while fetching user data." });
            }
        }
    }
}