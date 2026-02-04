using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class TasksController : Controller
{
    private TaskManager TaskManager { get; set; }
    public TasksController(TaskManager taskManager)
    {
        TaskManager = taskManager;
    }
    public async Task<string> Index()
    {
        var list = await TaskManager.List();
        return JsonConvert.SerializeObject(list, Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
    }

    [HttpPost]
    public async Task<IActionResult> Add(string taskName = "", string description = "")
    {
        await TaskManager.Add(taskName, description);
        return Ok();
    }
}