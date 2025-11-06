using CustomerInsight.GoogleAnalyticsWorker;
using CustomerInsight.GoogleAnalyticsWorker.Repositories;
using CustomerInsight.GoogleReview.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<TenantRepository>();
builder.Services.AddSingleton<ReviewSink>();
builder.Services.AddSingleton<GoogleTokenService>();
builder.Services.AddSingleton<BusinessProfileClient>();
builder.Services.AddHostedService<ReviewsIngestWorker>();
var host = builder.Build();
host.Run();
