using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagementApi.DTO;
using TaskManagementApi.Interface;
using TaskManagementApi.Models;

namespace TaskManagementApi.BLL;

public class AuthService : IAuthService
{
    private readonly TaskManagementDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(TaskManagementDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> RegisterAsync(RegisterDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return false;

        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            Passwordhash = "" // Will be hashed below
        };

        user.Passwordhash = passwordHasher.HashPassword(user, request.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
        if (role != null)
        {
            _context.Userroles.Add(new Userrole
            {
                Userid = user.Id,
                Roleid = role.Id
            });
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return null;

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.Passwordhash, request.Password);
        if (result == PasswordVerificationResult.Failed) return null;

        var userRole = await _context.Userroles
                            .Include(ur => ur.Role)
                            .FirstOrDefaultAsync(ur => ur.Userid == user.Id);

        var token = GenerateJwtToken(user, userRole?.Role?.Name ?? "User");

        return new AuthResponseDto
        {
            Token = token,
            Role = userRole?.Role?.Name ?? "User",
            Email = user.Email,
            Username = user.Username
        };
    }

    private string GenerateJwtToken(User user, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", user.Id.ToString())

        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

}
