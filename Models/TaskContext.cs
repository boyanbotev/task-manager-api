using Microsoft.EntityFrameworkCore;

public class TaskContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }

    public string DbPath { get; }
    public TaskContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "tasks.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}