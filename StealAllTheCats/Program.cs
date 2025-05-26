using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddSwaggerGen();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();