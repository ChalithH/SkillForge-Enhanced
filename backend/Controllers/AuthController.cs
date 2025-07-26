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

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                {
                    return Unauthorized();
                }

                var updatedUser = await _authService.UpdateProfileAsync(id, updateProfileDto);
                if (updatedUser == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id = updatedUser.Id,
                    email = updatedUser.Email,
                    name = updatedUser.Name,
                    timeCredits = updatedUser.TimeCredits,
                    bio = updatedUser.Bio,
                    profileImageUrl = updatedUser.ProfileImageUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return StatusCode(500, new { message = "An error occurred while updating profile." });
            }
        }

        [HttpPost("profile/image")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfileImage(IFormFile image)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                {
                    return Unauthorized();
                }

                if (image == null || image.Length == 0)
                {
                    return BadRequest(new { message = "No image file provided." });
                }

                var relativePath = await _authService.SaveProfileImageAsync(image, id);
                if (relativePath == null)
                {
                    return BadRequest(new { message = "Failed to save image." });
                }

                // Convert relative path to full URL
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fullImageUrl = $"{baseUrl}{relativePath}";

                // Update user's profile image URL
                var updateDto = new UpdateProfileDto
                {
                    Name = "", // Will be ignored since we're only updating the image
                    ProfileImageUrl = fullImageUrl
                };

                var user = await _authService.GetUserByIdAsync(id);
                if (user != null)
                {
                    updateDto.Name = user.Name;
                    updateDto.Bio = user.Bio;
                    await _authService.UpdateProfileAsync(id, updateDto);
                }

                return Ok(new { imageUrl = fullImageUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                return StatusCode(500, new { message = "An error occurred while uploading image." });
            }
        }
    }
}