using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var contextOptions = new DbContextOptionsBuilder<TaskContext>().UseSqlite().Options;
var db = new TaskContext(contextOptions);
db.Database.EnsureCreated();

builder.Logging.AddConsole();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddScoped<TaskService>();
builder.Services.AddDbContext<TaskContext>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
