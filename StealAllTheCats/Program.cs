using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using StealAllTheCats.Data;
using StealAllTheCats.Repositories;
using StealAllTheCats.Services;

var builder = WebApplication.CreateBuilder(args);

#region Services
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("CatApiClient", (serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["CatApi:BaseUrl"]);
    client.DefaultRequestHeaders.Add("x-api-key", configuration["CatApi:ApiKey"]);
});

builder.Services.AddScoped<ICatRepository, CatRepository>();
builder.Services.AddScoped<ICatService, CatService>();

builder.Services.AddHangfire(cfg =>
    cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
       .UseSimpleAssemblyNameTypeSerializer()
       .UseRecommendedSerializerSettings()
       .UseSqlServerStorage(
           builder.Configuration.GetConnectionString("DefaultConnection"),
           new SqlServerStorageOptions
           {
               CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
               SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
               QueuePollInterval = TimeSpan.Zero,
               UseRecommendedIsolationLevel = true,
               DisableGlobalLocks = true
           }
       )
);
builder.Services.AddHangfireServer();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "StealAllTheCats API", Version = "v1" });

    // Manually specify the server URL for Swagger UI
    c.AddServer(new OpenApiServer
    {
        Url = "http://localhost:5000"
    });
});

// Configure Serilog before building the app
//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()                            
//    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                                                                     // Default minimum level
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)                                      // Microsoft logs (EF Core, ASP.NET Core) only Warning+
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning) // EF Core SQL queries Warning+
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    db.Database.Migrate(); // ensures Catsdb + Hangfire tables are created
//}
#region DB Migration
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

var maxRetries = 10;
var retryCount = 0;
var delay = TimeSpan.FromSeconds(5);

while (retryCount < maxRetries)
{
    try
    {
        db.Database.Migrate();
        break;  // success
    }
    catch (Exception ex)
    {
        Log.Warning("Database migration failed. Retrying in {Delay}s... Exception: {Exception}", delay.TotalSeconds, ex.Message);
        await Task.Delay(delay);
        retryCount++;
    }
}

if (retryCount == maxRetries)
{
    Log.Error("Could not migrate the database after {MaxRetries} attempts.", maxRetries);
    throw new Exception("Database migration failed.");
}
#endregion

app.Run();