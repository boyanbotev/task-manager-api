public interface ITaskService
{
    Task<AddResult> Add(TaskItem task);
    Task<RemoveResult> Remove(string taskName);
    Task<TaskItem[]> List();
}

public enum RemoveResult
{
    Success,
    NotFound
}

public enum AddResult
{
    Success,
    AlreadyExists,
    Invalid
}

public class TaskService : ITaskService {
    private List<TaskItem> tasks = new List<TaskItem>();

    public async Task<AddResult> Add(TaskItem task)
    {
        if (string.IsNullOrEmpty(task.Name) || string.IsNullOrEmpty(task.Description))
        {
            return AddResult.Invalid;
        }

        if (tasks.Any(t => t.Name == task.Name))
        {
            return AddResult.AlreadyExists;
        }

        await Task.Delay(2000);

        tasks.Add(task);
        return AddResult.Success;
    }

    public async Task<TaskItem[]> List()
    {
        await Task.Delay(1000);
        return tasks.ToArray();
    }

    public async Task<RemoveResult> Remove(string taskName) 
    {
        if (!tasks.Any(t => t.Name == taskName))
        {
            return RemoveResult.NotFound;
        }
        await Task.Delay(2000);

        tasks.RemoveAll(task => task.Name == taskName);
        return RemoveResult.Success;
    }
}