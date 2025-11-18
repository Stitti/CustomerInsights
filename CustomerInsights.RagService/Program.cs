using System;
using CustomerInsights.RagService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CustomerInsights.RagService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = webApplicationBuilder.Configuration;

            webApplicationBuilder.Services.AddControllers();
            webApplicationBuilder.Services.AddEndpointsApiExplorer();
            webApplicationBuilder.Services.AddSwaggerGen();

            // HttpClients
            webApplicationBuilder.Services.AddHttpClient(
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

            webApplicationBuilder.Services.AddHttpClient(
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
            webApplicationBuilder.Services.AddScoped<EmbeddingClient>();
            webApplicationBuilder.Services.AddScoped<OllamaChatClient>();
            webApplicationBuilder.Services.AddScoped<InteractionRepository>();
            webApplicationBuilder.Services.AddScoped<RagQueryService>();

            WebApplication webApplication = webApplicationBuilder.Build();

            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
            }

            webApplication.UseAuthorization();
            webApplication.MapControllers();
            webApplication.Run();
        }
    }
}
