using Microsoft.AspNetCore.Mvc;
using WorkApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WorkApi.Services;
using System.IdentityModel.Tokens.Jwt;


[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private TaskService TaskService { get; set; }
    private readonly UserManager<User> userManager;
    public TasksController(TaskService taskService, UserManager<User> userManager, Settings settings)
    {
        TaskService = taskService;
        this.userManager = userManager;
    }

    public async Task<ActionResult> Index()
    {
        var userId = User.FindFirst("UserId")?.Value;
        Console.WriteLine("userId: " + userId);

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Key: {claim.Type}, Value: {claim.Value}");
        }

        var tasks = await TaskService.List(userId);
        return Ok(tasks);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddRequest task)
    {
        Console.WriteLine("users: " + userManager.Users.ToList().Count);
        foreach (var u in userManager.Users.ToList())
        {
            Console.WriteLine("userName: " + u.UserName);
            Console.WriteLine("id: " + u.Id);
        }

        var result = await TaskService.Add(task);
        switch (result)
        {
            case AddResult.Success:
                return Created();
            case AddResult.AlreadyExists:
                return Conflict();
            case AddResult.Invalid:
                return BadRequest();
            default:
                return StatusCode(500);
        }
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromBody] DeleteRequest deleteRequest)
    {
        var result = await TaskService.Remove(deleteRequest.Name);
        switch (result)
        {
            case RemoveResult.Success:
                return NoContent();
            case RemoveResult.NotFound:
                return NotFound();
            default:
                return StatusCode(500);
        }
    }
}