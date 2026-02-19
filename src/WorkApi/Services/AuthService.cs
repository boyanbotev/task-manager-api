using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace WorkApi.Services;

public enum LoginResult
{
    Success,
    Invalid
}

public enum RegisterResult
{
    Success,
    AlreadyExists
}

public class AuthService
{
    private readonly UserManager<User> userManager;
    private readonly Settings settings;
    private readonly TaskContext db;
    private readonly ILogger<TaskService> logger;

    public AuthService(UserManager<User> userManager, Settings settings, TaskContext db, ILogger<TaskService> logger)
    {
        this.userManager = userManager;
        this.settings = settings;
        this.db = db;
        this.logger = logger;
    }

    public async Task<string> Register(RegisterRequest registerRequest)
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

            await db.Users.AddAsync(user);

            try {
                await db.SaveChangesAsync();
            } catch (DbUpdateException) {
                logger.LogError("Username is not available.");
                return null;
            }

            return token;
        }

        return null; // not sure this is correct, what about our errors?
    }

    public async Task<string> Login(LoginRequest loginRequest)
    {
        var user = await userManager.FindByNameAsync(loginRequest.Username);

        if (user != null && await userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            var token = GenerateJwtToken(user.UserName, user.Id);
            return token;
        }
        return null;  // not sure this is correct, what about our errors?
    }

    private string GenerateJwtToken(string username, string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim("UserId", userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        Console.WriteLine("Bearer key: " + settings.BearerKey);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.BearerKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}