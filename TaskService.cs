public interface ITaskService
{
    Task Add(TaskItem task);
    Task Remove(string taskName);
    Task<TaskItem[]> List();
}

public class TaskService : ITaskService {
    private List<TaskItem> tasks = new List<TaskItem>();

    public async Task Add(TaskItem task)
    {
        if (string.IsNullOrEmpty(task.Name) || string.IsNullOrEmpty(task.Description))
        {
            return;
            // TODO: error handling
        }

        if (tasks.Any(t => t.Name == task.Name))
        {
            return;
            // TODO: error handling
        }

        await Task.Delay(2000);

        tasks.Add(task);
    }

    public async Task<TaskItem[]> List()
    {
        await Task.Delay(1000);
        return tasks.ToArray();
    }

    public async Task Remove(string taskName) 
    {
        if (!tasks.Any(t => t.Name == taskName))
        {
            return;
        }
        await Task.Delay(2000);

        tasks.RemoveAll(task => task.Name == taskName);
    }
}