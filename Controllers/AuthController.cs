using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.DTO;
using TaskManagementApi.Interface;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result)
            return BadRequest("Email already exists");

        return Ok(new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized("Invalid credentials");

        // Set JWT token as HttpOnly cookie
        HttpContext.Response.Cookies.Append("jwtToken", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // set to true in production
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60) // match your token expiry
        });

        return Ok(new
        {
            role = response.Role,
            email = response.Email,
            username = response.Username
        });
    }
    [Authorize]
    [HttpGet("validate")]
    public IActionResult ValidateToken()
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { message = "Token is valid", role });
    }
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwtToken");
        return Ok(new { message = "Logged out" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _authService.DeleteTaskAsync(id);
        if (task == null)
            return NotFound("Task not found");
        return Ok(new { message = "Task deleted successfully" });
    }
    
}
