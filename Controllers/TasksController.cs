using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class TasksController : Controller
{
    public string Index()
    {
        return "Hello World!";
    }

    [HttpPost]
    public async Task<IActionResult> Add(string taskName = "", string description = "")
    {
        await Task.Delay(1000);
        TaskItem task = new TaskItem(taskName, description);
        return Ok(task);
    }
}