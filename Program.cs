var builder = WebApplication.CreateBuilder(args);

var TaskManager = new TaskManager();
builder.Services.AddControllers();
builder.Services.AddSingleton(TaskManager);

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
