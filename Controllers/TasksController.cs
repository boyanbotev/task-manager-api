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
    public async Task<IActionResult> Add([FromBody] TaskItem task)
    {
        var result = await TaskService.Add(task);
        if (result == AddResult.AlreadyExists)
        {
            return Conflict();
        }
        if (result == AddResult.Invalid)
        {
            return BadRequest();
        }
        return Created();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromBody] DeleteRequest deleteRequest)
    {
        var result = await TaskService.Remove(deleteRequest.Name);
        if (result == RemoveResult.NotFound)
        {
            return NotFound();
        }
        return NoContent();
    }
}