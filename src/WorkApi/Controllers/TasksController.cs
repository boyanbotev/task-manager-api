using Microsoft.AspNetCore.Mvc;
using WorkApi.Models;
using Microsoft.AspNetCore.Authorization;
using WorkApi.Services;

[ApiController]
[Route("tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService taskService;
    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var userId = User.FindFirst("UserId")?.Value;
        var tasks = await taskService.List(userId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddRequest task)
    {
        var userId = User.FindFirst("UserId")?.Value;
        task.UserId = userId;
        var result = await taskService.Add(task);
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        string userId = User.FindFirst("UserId")?.Value;
        var result = await taskService.Remove(id, userId);
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