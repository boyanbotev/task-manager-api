var builder = WebApplication.CreateBuilder(args);

var db = new TaskContext();
db.Database.EnsureCreated();

var taskService = new TaskService(db);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddSingleton(taskService);
builder.Services.AddDbContext<TaskContext>();

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
