using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using WorkApi.Models;
using WorkApi.Services;

var builder = WebApplication.CreateBuilder(args);

var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

var folder = Environment.SpecialFolder.LocalApplicationData;
var path = Environment.GetFolderPath(folder);
var dbPath = System.IO.Path.Join(path, "tasks.db");

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddIdentityCore<User>(options => 
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TaskContext>();

builder.Logging.AddConsole();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.BearerKey)),
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskContext>();
    db.Database.Migrate();
}

// tests
// if I use Database.Migrate() running tests fails with: 'no such table: __EFMigrationsHistory'.
// if I use Database.EnsureCreated() running tests fails with: 'no such table: AspNetUsers'.
// if I have nothing here running tests fails with: 'no such table: AspNetUsers'.
// i cannot understand what is going on.
// perhaps the test is just setup incorrectly and I need to copy from github


app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }
