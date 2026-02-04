public class TaskItem
{
    public string TaskName { get; set; }
    public string Description { get; set; }

    public TaskItem(string taskName, string description)
    {
        TaskName = taskName;
        Description = description;
    }
}
