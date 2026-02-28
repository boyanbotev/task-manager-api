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
    public async Task<ActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("UserId")?.Value;
        var tasks = await taskService.List(userId, cancellationToken);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddRequest task, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("UserId")?.Value;
        var result = await taskService.Add(userId, task, cancellationToken);
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
    public async Task<IActionResult> Remove([FromRoute] int id, CancellationToken cancellationToken)
    {
        string userId = User.FindFirst("UserId")?.Value;
        var result = await taskService.Remove(id, userId, cancellationToken);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateRequest task, CancellationToken cancellationToken)
    {
        string userId = User.FindFirst("UserId")?.Value;
        var result = await taskService.Update(id, userId, task, cancellationToken);
        switch (result)
        {
            case UpdateResult.Success:
                return NoContent();
            case UpdateResult.NotFound:
                return NotFound();
            case UpdateResult.Invalid:
                return BadRequest();
            default:
                return StatusCode(500);
        }
    }
}