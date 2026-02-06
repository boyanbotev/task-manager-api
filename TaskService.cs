using Microsoft.EntityFrameworkCore;
public interface ITaskService
{
    Task<AddResult> Add(AddRequest task);
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
    private readonly TaskContext db;

    public TaskService(TaskContext db)
    {
        this.db = db;
    }

    public async Task<AddResult> Add(AddRequest task)
    {
        if (string.IsNullOrEmpty(task.Name) || string.IsNullOrEmpty(task.Description))
        {
            return AddResult.Invalid;
        }

        var dbTask = await db.Tasks.FirstOrDefaultAsync(t => t.Name == task.Name);
        if (dbTask != null)
        {
            return AddResult.AlreadyExists;
        }

        await db.Tasks.AddAsync(new TaskItem
        {
            Name = task.Name,
            Description = task.Description
        });
        await db.SaveChangesAsync();

        return AddResult.Success;
    }

    public async Task<TaskItem[]> List()
    {
        var tasks = await db.Tasks.ToListAsync();
        return tasks.ToArray();
    }

    public async Task<RemoveResult> Remove(string taskName) 
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Name == taskName);
        if (task == null)
        {
            return RemoveResult.NotFound;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return RemoveResult.Success;
    }
}