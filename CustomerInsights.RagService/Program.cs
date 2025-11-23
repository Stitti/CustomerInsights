using System;
using CustomerInsights.Database;
using CustomerInsights.RagService.Services;
using CustomerInsights.ServiceDefaults;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Serilog;

namespace CustomerInsights.RagService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddServiceDefaults();

            IConfiguration configuration = builder.Configuration;
            builder.AddLogging();

            string? connectionString = builder.Configuration.GetConnectionString("customer-insights-db");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("Database connection string is missing on configuration");

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.UseVector();
                    });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal API", Version = "v1" });
            });

            // HttpClients
            builder.Services.AddHttpClient(
                "EmbeddingClient",
                httpClient =>
                {
                    string? baseUrlFromConfig = configuration["EmbeddingService:BaseUrl"];
                    if (string.IsNullOrWhiteSpace(baseUrlFromConfig))
                    {
                        throw new InvalidOperationException("EmbeddingService:BaseUrl is not configured.");
                    }

                    httpClient.BaseAddress = new Uri(baseUrlFromConfig);
                });

            builder.Services.AddHttpClient(
                "ChatClient",
                httpClient =>
                {
                    string? baseUrlFromConfig = configuration["ChatService:BaseUrl"];
                    if (string.IsNullOrWhiteSpace(baseUrlFromConfig))
                    {
                        throw new InvalidOperationException("ChatService:BaseUrl is not configured.");
                    }

                    httpClient.BaseAddress = new Uri(baseUrlFromConfig);
                });

            // Services
            builder.Services.AddScoped<EmbeddingClient>();
            builder.Services.AddScoped<OllamaChatClient>();
            builder.Services.AddScoped<InteractionEmbeddingRepository>();
            builder.Services.AddScoped<RagQueryService>();

            WebApplication webApplication = builder.Build();

            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
            }

            webApplication.UseAuthorization();
            webApplication.MapControllers();
            try
            {
                Log.Information("Starting application...");
                await webApplication.RunAsync();
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
}
