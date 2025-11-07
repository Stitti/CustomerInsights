using CustomerInsights.ApiService.Database;
using CustomerInsights.ApiService.Database.Repositories;
using CustomerInsights.ApiService.Repositories;
using CustomerInsights.ApiService.Services;
using CustomerInsights.ApiService.Utils;
using CustomerInsights.Database;
using CustomerInsights.ServiceDefaults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Npgsql;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.AddServiceDefaults();

string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("Database connection string is missing on configuration");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<InteractionRepository>();
builder.Services.AddScoped<ContactRepository>();
builder.Services.AddScoped<AccountRepository>();

// Services
builder.Services.AddSingleton<TextNormalizer>();
builder.Services.AddScoped<InteractionService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<AccountService>();

builder.AddLogging();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal API", Version = "v1" });
    //o.AddServer(new OpenApiServer { Url = "http://localhost:5200" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
if (dbContext.Database.GetPendingMigrations().Any())
{
    dbContext.Database.Migrate();
}

app.UseCors("AllowAll");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

try
{
    Log.Information("Starting application...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application terminated unexpectedly.");
    throw;
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}