using CustomerInsights.Database;
using CustomerInsights.SatisfactionIndexService.Models;
using CustomerInsights.SatisfactionIndexService.Repositories;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CustomerInsights.SatisfactionIndexService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.Configure<RabbitMqConnectionOptions>(builder.Configuration.GetSection("RabbitMq"));

        string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Database connection string is missing on configuration");

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);

        builder.AddLogging();
        builder.Services.AddSingleton<AggregationConfig>();
        builder.Services.AddSingleton<Weights>();
        builder.Services.AddSingleton<InteractionRepository>();
        builder.Services.AddSingleton<SatisfactionStateRepository>();
        builder.Services.AddSingleton<SatisfactionIndexRecalculationService>();
        builder.Services.AddHostedService<Worker>();

        IHost host = builder.Build();
        try
        {
            Log.Information("Starting application...");
            host.Run();
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
    }
}
