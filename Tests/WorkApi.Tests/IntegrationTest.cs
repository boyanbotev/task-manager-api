using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkApi.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

public class TasksControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TasksControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var _dbPath = Path.Join(Environment.GetFolderPath(folder), $"tests_{Guid.NewGuid()}.db");
        var dbConnectionString = $"Data Source={_dbPath}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // 2. Remove the existing DbContextOptions from Program.cs
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TaskContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 3. Add the Test DbContext
                services.AddDbContext<TaskContext>(options =>
                    options.UseSqlite(dbConnectionString));
                
                // DO NOT call BuildServiceProvider() here.
            });
        });

        _client = _factory.CreateClient();

        // 4. Initialize the Test Database using the factory's actual Service Provider
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
        }
    }

    [Fact]
    public async Task ShouldGetEmptyListWhenNoTasksExist()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/tasks");

        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task ShouldAddAndRemoveTask()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        string userId = GetUserId("testuser");

        var taskRequest = new AddRequest 
        { 
            UserId = userId, 
            Name = "Test Task", 
            Description = "Description" 
        };

        var content = new StringContent(JsonSerializer.Serialize(taskRequest), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/tasks", content);

        response.EnsureSuccessStatusCode();

        var tasksResponse = await _client.GetAsync("/tasks");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks);
        Assert.Equal("Test Task", tasks.First().Name);

        var deleteResponse = await _client.DeleteAsync("/tasks/1");

        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var tasksResponse2 = await _client.GetAsync("/tasks");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks2 = await tasksResponse2.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Empty(tasks2);
    }


    private async Task<string> GetAuthToken(string username, string password)
    {
        var registerRequest = new { username, password };
        var registerContent = new StringContent(JsonSerializer.Serialize(registerRequest), Encoding.UTF8, "application/json");
        var registerResponse = await _client.PostAsync("/auth/register", registerContent);

        var loginRequest = new { username, password };
        var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
        var loginResponse = await _client.PostAsync("/auth/login", loginContent);
        loginResponse.EnsureSuccessStatusCode();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        return loginResult["token"];
    }

    private string GetUserId(string username)
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = userManager.FindByNameAsync(username).Result;
            return user.Id;
        }
    }

    public void Dispose()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TaskContext>();
            db.Database.CloseConnection();
        }
    }
}
