using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillForge.Api.Data;
using SkillForge.Api.DTOs.Auth;
using SkillForge.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillForge.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(User user);
        Task<User?> GetUserByIdAsync(int id);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null;
            }

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                Bio = registerDto.Bio,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                TimeCredits = 5, // New users start with 5 free credits
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token and return response
            var token = GenerateJwtToken(user);
            
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                TimeCredits = user.TimeCredits,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate token and return response
            var token = GenerateJwtToken(user);
            
            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Id = user.Id,
                TimeCredits = user.TimeCredits,
                ProfileImageUrl = user.ProfileImageUrl
            };
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]!);
            var expirationHours = int.Parse(_configuration["JwtSettings:ExpirationInHours"] ?? "24");
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(expirationHours),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}