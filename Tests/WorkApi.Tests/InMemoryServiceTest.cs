using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests;

public class InMemoryServiceTest
{
    TaskContext context;
    public InMemoryServiceTest()
    {
        var contextOptions = new DbContextOptionsBuilder<TaskContext>()
            .UseInMemoryDatabase("TestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;

        context = new TaskContext(contextOptions);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.SaveChanges();
    }

    // Sample test method
    [Fact]
    public async Task ShouldPopulateDb()
    {
        var taskService = new TaskService(context, new NullLogger<TaskService>());
        await taskService.Add(new AddRequest
        {
            Name = "Test",
            Description = "Test"
        });
        await taskService.Add(new AddRequest
        {
            Name = "Test2",
            Description = "Test2"
        });

        await context.SaveChangesAsync();

        var tasks = await taskService.List();
        Assert.Equal(2, tasks.Length);
    }
}

