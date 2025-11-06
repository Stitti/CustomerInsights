using CustomerInsights.Database;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.SignalWorker.Models;
using CustomerInsights.SignalWorker.Repositories;
using CustomerInsights.SignalWorker.Workers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CustomerInsights.SignalWorker;

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
        builder.Services.AddSingleton<OutboxRepository>();
        builder.Services.AddSingleton<SignalRepository>();
        builder.Services.AddSingleton<SatisfactionStateRepository>();
        builder.Services.AddSingleton<SiBelowTresholdSignalConfig>();
        builder.Services.AddHostedService<SiBelowThresholdWorker>();
        builder.AddLogging();

        IHost host = builder.Build();
        host.Run();
    }
}