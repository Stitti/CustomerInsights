using CustomerInsights.Database;
using CustomerInsights.NlpService;
using CustomerInsights.NlpService.Repositories;
using CustomerInsights.NlpService.Runtime;
using CustomerInsights.NlpService.Services;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace CustomerInsights.EmailService;

public sealed class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.Configure<RabbitMqConnectionOptions>(builder.Configuration.GetSection("RabbitMq"));

        string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("Database connection string is missing on configuration");
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        builder.AddLogging();

        builder.Services.AddHttpClient("PresidioAnalyzer", httpClient =>
        {
            string? analyzerUrl = builder.Configuration["Presidio:AnalyzerUrl"];
            if (string.IsNullOrWhiteSpace(analyzerUrl))
            {
                throw new InvalidOperationException("Presidio Analyzer URL is not configured (Presidio:AnalyzerUrl).");
            }

            httpClient.BaseAddress = new Uri(analyzerUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient("PresidioAnonymizer", httpClient =>
        {
            string? anonymizerUrl = builder.Configuration["Presidio:AnonymizerUrl"];
            if (string.IsNullOrWhiteSpace(anonymizerUrl))
            {
                throw new InvalidOperationException("Presidio Anonymizer URL is not configured (Presidio:AnonymizerUrl).");
            }

            httpClient.BaseAddress = new Uri(anonymizerUrl);
            httpClient.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddSingleton<PresidioService>(serviceProvider =>
        {
            IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            HttpClient analyzerHttpClient = httpClientFactory.CreateClient("PresidioAnalyzer");
            HttpClient anonymizerHttpClient = httpClientFactory.CreateClient("PresidioAnonymizer");
            ILogger<PresidioService> logger = serviceProvider.GetRequiredService<ILogger<PresidioService>>();

            PresidioService presidioService = new PresidioService(analyzerHttpClient, anonymizerHttpClient, logger);
            return presidioService;
        });

        builder.Services.AddSingleton<TextInferenceRepository>();
        builder.Services.AddSingleton<PresidioService>();
        builder.Services.AddSingleton<IdentityResolvingService>();
        builder.Services.AddSingleton<ZeroShotAspectNliOnnx>();
        builder.Services.AddSingleton<SentimentOnnx3>();
        builder.Services.AddSingleton<EmotionOnnxMulti>();
        builder.Services.AddSingleton<UrgencyOnnx3>();
        builder.Services.AddSingleton<TextAnalyzer>();
        builder.Services.AddSingleton<TextInferenceService>();
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