using CustomerInsights.Database;
using CustomerInsights.InferenceWorker.Repositories;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.Worker.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CustomerInsights.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("Database connection string is missing on configuration");

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);
        builder.Services.AddSingleton<InteractionRepository>();
        builder.Services.AddSingleton<OutboxRepository>();
        builder.Services.AddSingleton<SatisfactionStateRepository>();
        builder.Services.AddSingleton<TextInferenceRepository>();
        builder.Services.AddSingleton<NlpClient>();

        builder.Services.AddHostedService<InteractionProcessingWorker>();
        
        builder.AddLogging();

        IHost host = builder.Build();
        host.Run();
    }
}