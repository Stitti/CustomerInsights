using AspNetCoreRateLimit;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using Serilog;
using Serilog.Extensions;
using Serilog.Formatting.Compact;

namespace CustomerInsights.ServiceDefaults
{
    public static class BuilderExtensions
    {
        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "EventManager")
                .Enrich.WithClientIp()
                .Enrich.WithRequestBody()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj} (TraceId={TraceId}){NewLine}{Exception}")
                .WriteTo.File(path: "Logs/log.json", rollingInterval: RollingInterval.Day, formatter: new CompactJsonFormatter())
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog();
            return builder;
        }

        public static HostApplicationBuilder AddLogging(this HostApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj} (TraceId={TraceId}){NewLine}{Exception}")
                .WriteTo.File(path: "Logs/log.json", rollingInterval: RollingInterval.Day, formatter: new CompactJsonFormatter())
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);
            return builder;
        }

        public static WebApplicationBuilder AddKeyVaultService(this WebApplicationBuilder builder)
        {
            string? vaultUri = builder.Configuration.GetValue<string>("KeyVault:VaultUri");
            if (string.IsNullOrWhiteSpace(vaultUri))
                throw new InvalidOperationException("KeyVault:VaultUri is required.");

            builder.Services.AddSingleton<TokenCredential>(_ => new DefaultAzureCredential());
            builder.Services.AddSingleton(sp =>
            {
                TokenCredential? cred = sp.GetRequiredService<TokenCredential>();
                SecretClientOptions options = new SecretClientOptions
                {
                    Retry =
                    {
                        Mode = RetryMode.Exponential,
                        MaxRetries = 5,
                        Delay = TimeSpan.FromMilliseconds(500),
                        MaxDelay = TimeSpan.FromSeconds(5)
                    },
                    Diagnostics =
                    {
                        IsLoggingEnabled = true,
                        IsLoggingContentEnabled = false
                    }
                };

                return new SecretClient(new Uri(vaultUri), cred, options);
            });

            return builder;
        }

        public static WebApplicationBuilder AddRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = 429;
                options.QuotaExceededResponse = new QuotaExceededResponse
                {
                    Content = "{{\"message\": \"Too many requests. Please try again later.\"}}",
                    ContentType = "application/json",
                    StatusCode = 429
                };
            });
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            builder.Services.AddInMemoryRateLimiting();
            return builder;
        }

        public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
        {
            builder.ConfigureOpenTelemetry();

            builder.AddDefaultHealthChecks();

            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

            return builder;
        }

        public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSource(builder.Environment.ApplicationName)
                        .AddAspNetCoreInstrumentation()
                        // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                        //.AddGrpcClientInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.AddOpenTelemetryExporters();

            return builder;
        }

        private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            bool useOtlpExporter = string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]) == false;

            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

            // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
            //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            //{
            //    builder.Services.AddOpenTelemetry()
            //       .UseAzureMonitor();
            //}

            return builder;
        }

        public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
            where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddHealthChecks()
                            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapHealthChecks("/health");
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });
            }

            return app;
        }

        public static IServiceCollection AddRabbitMqSender(this IServiceCollection services, IConfiguration configuration, string connectionName)
        {
            services.AddSingleton<IConnection>(serviceProvider =>
            {
                IConfiguration rabbitMqSection = configuration.GetSection(connectionName);
                string? connectionString = rabbitMqSection.Get<string>();

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("RabbitMQ connection string not found for name: " + connectionName);
                }

                Uri connectionUri = new Uri(connectionString);

                ConnectionFactory connectionFactory = new ConnectionFactory
                {
                    HostName = connectionUri.Host,
                    Port = connectionUri.Port,
                    UserName = connectionUri.UserInfo.Split(':')[0],
                    Password = connectionUri.UserInfo.Split(':')[1],
                    VirtualHost = connectionUri.AbsolutePath.TrimStart('/'),
                };

                Task<IConnection> connectionTask = connectionFactory.CreateConnectionAsync();
                connectionTask.Wait();

                return connectionTask.Result;
            });

            services.AddSingleton<RabbitMqSenderService>();
            return services;
        }
    }
}