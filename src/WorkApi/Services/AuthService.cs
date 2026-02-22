using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace WorkApi.Services;

public interface IAuthService
{
    public Task<(bool Succeeded, string Token, IEnumerable<string> Errors)> Register(RegisterRequest registerRequest);
    public Task<(bool Succeeded, string Token)> Login(LoginRequest loginRequest);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> userManager;
    private readonly Settings settings;
    private readonly ILogger<AuthService> logger;

    public AuthService(UserManager<User> userManager, Settings settings, ILogger<AuthService> logger)
    {
        this.userManager = userManager;
        this.settings = settings;
        this.logger = logger;
    }

    public async Task<(bool Succeeded, string Token, IEnumerable<string> Errors)> Register(RegisterRequest registerRequest)
    {
        var user = new User
        {
            UserName = registerRequest.Username,
        };

        var result = await userManager.CreateAsync(user, registerRequest.Password);

        foreach (var error in result.Errors)
        {
            logger.LogError(error.Description);
        }
        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user.UserName, user.Id);
            return (true, token, null);
        }

        return (false, null,  result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Succeeded, string Token)> Login(LoginRequest loginRequest)
    {
        var user = await userManager.FindByNameAsync(loginRequest.Username);

        if (user != null && await userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            var token = GenerateJwtToken(user.UserName, user.Id);
            return (true, token);
        }
        return (false, null);
    }

    private string GenerateJwtToken(string username, string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim("UserId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.BearerKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}