using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkApi.Models;
using WorkApi.Services;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly Settings settings;
    private readonly AuthService authService;

    public AuthController(UserManager<User> userManager, Settings settings, AuthService authService)
    {
        this.userManager = userManager;
        this.settings = settings; 
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        // var user = new User
        // {
        //     UserName = registerRequest.Username,
        // };

        // var result = await userManager.CreateAsync(user, registerRequest.Password);
        // if (result.Succeeded)
        // {
        //     var token = GenerateJwtToken(user.UserName);
        //     return Ok(new { token });
        // }

        var token = await authService.Register(registerRequest);
        if (token != null)
        {
            return Ok(new { token });
        }
        return BadRequest();

        // TODO: we have to add the user to the db
        // we need an authentication service
        //return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var token = await authService.Login(loginRequest);

        if (token != null)
        {
            return Ok(new { token });
        }
        // var user = await userManager.FindByNameAsync(loginRequest.Username);

        // if (user != null && await userManager.CheckPasswordAsync(user, loginRequest.Password))
        // {
        //     var token = GenerateJwtToken(user.UserName);
        //     return Ok(new { token });
        // }
        return Unauthorized();
    }

    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
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