using FarmManagement.Web.Data;
using FarmManagement.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FarmManagement.PestResourceService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FarmDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(FarmDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    /// <summary>Obtain a JWT token. Use admin@farmmanagement.com / Admin@123</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == req.Email && !u.IsBlocked);
        if (user == null || !PasswordHelper.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddHours(double.Parse(_config["Jwt:ExpiryHours"]!));

        var token = new JwtSecurityToken(
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name,           user.FullName),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           user.Role.ToString())
            },
            expires: expiry,
            signingCredentials: creds);

        return Ok(new
        {
            token   = new JwtSecurityTokenHandler().WriteToken(token),
            expires = expiry,
            user    = new { user.FullName, user.Email, role = user.Role.ToString() }
        });
    }
}

public record LoginRequest(string Email, string Password);
