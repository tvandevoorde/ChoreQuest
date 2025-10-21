using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChoreQuest.Api.Data;
using ChoreQuest.Api.DTOs;
using ChoreQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChoreQuest.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return null;
        }

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return null;
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return null;
        }

        return new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserProfileResponse?> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            // Check if email is already taken by another user
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Id != userId);
            if (existingUser != null)
            {
                return null;
            }

            user.Email = request.Email;
        }

        await _context.SaveChangesAsync();

        return new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "ChoreQuestDefaultSecretKeyForDevelopment123456";
        var issuer = jwtSettings["Issuer"] ?? "ChoreQuest";
        var audience = jwtSettings["Audience"] ?? "ChoreQuestUsers";

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
