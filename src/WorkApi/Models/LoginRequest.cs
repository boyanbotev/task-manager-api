using System.ComponentModel.DataAnnotations;

namespace WorkApi.Models;

public class LoginRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }

    public LoginRequest() { }
}