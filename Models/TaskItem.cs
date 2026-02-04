public class TaskItem
{
    public string Name { get; set; }
    public string Description { get; set; }

    public TaskItem(string taskName, string description)
    {
        Name = taskName;
        Description = description;
    }
}
