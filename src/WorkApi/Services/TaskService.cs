using Microsoft.EntityFrameworkCore;
using WorkApi.Models;

namespace WorkApi.Services;

public interface ITaskService
{
    Task<AddResult> Add(AddRequest task);
    Task<RemoveResult> Remove(int id, string userId);
    Task<TaskItem[]> List(string userId);
    Task<UpdateResult> Update(int id, UpdateRequest task);
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

public enum UpdateResult
{
    Success,
    NotFound,
    Invalid
}

public class TaskService : ITaskService {
    private readonly TaskContext db;
    private readonly ILogger<TaskService> logger;

    public TaskService(TaskContext db, ILogger<TaskService> logger)
    {
        this.db = db;
        this.logger = logger;
    }

    public async Task<AddResult> Add(AddRequest task)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == task.UserId);

        await db.Tasks.AddAsync(new TaskItem
        {
            Name = task.Name,
            Description = task.Description,
            User = user
        });

        try {
            await db.SaveChangesAsync();
        } catch (DbUpdateException) {
            logger.LogError("Task already exists: {task}", task.Name);
            return AddResult.AlreadyExists;
        }

        logger.LogInformation("Task added: {task}", task.Name);

        return AddResult.Success;
    }

    public async Task<TaskItem[]> List(string userId)
    {
        var tasks = await db.Tasks.AsNoTracking()
            .Where(t => t.UserId== userId)
            .ToListAsync();

        logger.LogInformation("Tasks loaded: {tasks}", tasks.Count);
        return tasks.ToArray();
    }

    public async Task<RemoveResult> Remove(int id, string userId) 
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (task == null)
        {
            return RemoveResult.NotFound;
        }

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        logger.LogInformation("Task removed: {task}", task.Name);
        return RemoveResult.Success;
    }

    public async Task<UpdateResult> Update(int id, UpdateRequest task)
    {
        var taskItem = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == task.UserId);
        if (taskItem == null)
        {
            return UpdateResult.NotFound;
        }
        logger.LogInformation($"Changing task name from {taskItem.Name} to {task.Name}");
        logger.LogInformation($"Changing task description from {taskItem.Description} to {task.Description}");

        taskItem.Name = task.Name;
        taskItem.Description = task.Description;
        await db.SaveChangesAsync();
        return UpdateResult.Success;
    }
}