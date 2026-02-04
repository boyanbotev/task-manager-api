public class TaskManager {
    private List<TaskItem> tasks = new List<TaskItem>();

    public async Task Add(string name, string description)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
        {
            return;
        }

        if (tasks.Any(task => task.Name == name))
        {
            return;
        }

        await Task.Delay(2000);

        tasks.Add(new TaskItem(name, description));
    }

    public async Task<TaskItem[]> List()
    {
        await Task.Delay(1000);
        return tasks.ToArray();
    }

    public async Task Remove(string taskName)
    {
        if (!tasks.Any(task => task.Name == taskName))
        {
            return;
        }
        await Task.Delay(2000);

        tasks.RemoveAll(task => task.Name == taskName);
    }
}