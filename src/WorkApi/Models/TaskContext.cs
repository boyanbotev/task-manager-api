using Microsoft.EntityFrameworkCore;

public class TaskContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }

    public string DbPath { get; }
    public TaskContext(DbContextOptions<TaskContext> options) : base(options)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "tasks.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured) {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}