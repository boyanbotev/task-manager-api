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
        Console.WriteLine("list");
        var list = await TaskService.List();
        return Ok(list);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] TaskItem task)
    {
        Console.WriteLine("Adding task");
        await TaskService.Add(task);
        return Ok();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove([FromBody] DeleteRequest deleteRequest)
    {
        Console.WriteLine("Removing task");
        await TaskService.Remove(deleteRequest.Name);
        return Ok();
    }
}