using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkApi.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Net;

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
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TaskContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<TaskContext>(options =>
                    options.UseSqlite(dbConnectionString));
            });
        });

        _client = _factory.CreateClient();

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
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/tasks");

        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task ShouldAddAndRemoveTask()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        var client = CreateAuthenticatedClient(token);

        var taskRequest = new AddRequest 
        { 
            Name = "Test Task", 
            Description = "Description" 
        };

        var content = new StringContent(JsonSerializer.Serialize(taskRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/tasks", content);

        response.EnsureSuccessStatusCode();

        var tasksResponse = await client.GetAsync("/tasks");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks);
        Assert.Equal("Test Task", tasks.First().Name);

        var deleteResponse = await client.DeleteAsync("/tasks/1");

        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var tasksResponse2 = await client.GetAsync("/tasks");
        tasksResponse2.EnsureSuccessStatusCode();
        var tasks2 = await tasksResponse2.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Empty(tasks2);
    }

    [Fact]
    public async Task ShouldUpdateTask()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        var client = CreateAuthenticatedClient(token);

        var taskRequest = new AddRequest 
        { 
            Name = "Test Task", 
            Description = "Description" 
        };

        var content = new StringContent(JsonSerializer.Serialize(taskRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/tasks", content);

        response.EnsureSuccessStatusCode();

        var tasksResponse = await client.GetAsync("/tasks");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks);
        Assert.Equal("Test Task", tasks.First().Name);

        var updateRequest = new UpdateRequest 
        { 
            Name = "Updated Task", 
            Description = "Updated Description" 
        };
        var updateContent = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
        var updateResponse = await client.PutAsync($"/tasks/{tasks.First().Id}", updateContent);
        updateResponse.EnsureSuccessStatusCode();

        var tasksResponse2 = await client.GetAsync("/tasks");
        tasksResponse2.EnsureSuccessStatusCode();
        var tasks2 = await tasksResponse2.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.Single(tasks2);
        Assert.Equal("Updated Task", tasks2.First().Name);
    }

    [Fact]
    public async Task ShouldNotUpdateOtherUserTasks()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        string token2 = await GetAuthToken("testuser2", "Password123!");
        var client = CreateAuthenticatedClient(token);
        var client2 = CreateAuthenticatedClient(token2);

        var addRequest = new AddRequest 
        { 
            Name = "Task We Should Not Update",
            Description = "Description" 
        };
        var content = new StringContent(JsonSerializer.Serialize(addRequest), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/tasks", content);
        response.EnsureSuccessStatusCode();
        
        var tasksResponse = await client.GetAsync("/tasks");
        tasksResponse.EnsureSuccessStatusCode();
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskItem>>();

        var existingTaskId = tasks.Find(t => t.Name == addRequest.Name).Id;

        var updateRequest = new UpdateRequest 
        { 
            Name = "Updated Task", 
            Description = "Updated Description"
        };

        var updateContent = new StringContent(JsonSerializer.Serialize(updateRequest), Encoding.UTF8, "application/json");
        var updateResponse = await client2.PutAsync($"/tasks/{existingTaskId}", updateContent);
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);

        var tasksResponse2 = await client.GetAsync("/tasks");
        tasksResponse2.EnsureSuccessStatusCode();
        var tasks2 = await tasksResponse2.Content.ReadFromJsonAsync<List<TaskItem>>();

        var unchangedTask = tasks2.FirstOrDefault(t => t.Id == existingTaskId);
        Assert.NotNull(unchangedTask);
        Assert.Equal("Task We Should Not Update", unchangedTask.Name);
    }

    [Fact]
    public async Task ShouldRejectAnonymousRequests()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ShouldRejectDuplicateTaskNames()
    {
        string token = await GetAuthToken("testuser", "Password123!");
        var client = CreateAuthenticatedClient(token);

        var taskRequest = new AddRequest 
        { 
            Name = "Do Not Duplicate",
            Description = "Description" 
        };

        var content = new StringContent(JsonSerializer.Serialize(taskRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/tasks", content);
        response.EnsureSuccessStatusCode();

        var response2 = await client.PostAsync("/tasks", content);

        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
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

    private HttpClient CreateAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
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
