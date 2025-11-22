using CustomerInsights.Database;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.WebhookService.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CustomerInsights.WebhookService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Database connection string is missing on configuration");

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        builder.AddLogging();
        builder.Services.AddSingleton<WebhookSenderService>();
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
