using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WorkApi.Models;
using WorkApi.Services;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService authService;

    public AuthController(UserManager<User> userManager, AuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        var (succeeded, token, errors) = await authService.Register(registerRequest);
        if (succeeded)
        {
            return Ok(new { token });
        }
        return BadRequest(errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var (succeeded, token) = await authService.Login(loginRequest);

        if (succeeded)
        {
            return Ok(new { token });
        }

        return Unauthorized();
    }
}