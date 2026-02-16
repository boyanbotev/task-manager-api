using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private TaskService TaskService { get; set; }
    public TasksController(TaskService taskService)
    {
        TaskService = taskService;
    }

    public async Task<ActionResult> Index()
    {
        var list = await TaskService.List();
        return Ok(list);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddRequest task)
    {
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