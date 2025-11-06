using CustomerInsight.GoogleAnalyticsWorker;
using CustomerInsight.GoogleAnalyticsWorker.Models;
using CustomerInsight.GoogleAnalyticsWorker.Repositories;
using CustomerInsight.GoogleAnalyticsWorker.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<GoogleOAuthSection>(builder.Configuration.GetSection("GoogleOAuth"));
builder.Services.Configure<IngestOptions>(builder.Configuration.GetSection("Ingest"));

builder.Services.AddSingleton<TenantRepository>();
builder.Services.AddSingleton<WebsiteReportRepository>();
builder.Services.AddSingleton<GoogleTokenService>();
builder.Services.AddSingleton<GaDataClient>();

builder.Services.AddHostedService<AnalyticsIngestWorker>();

IHost host = builder.Build();
host.Run();
