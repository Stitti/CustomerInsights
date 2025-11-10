using CustomerInsights.WebhookWorker;
using CustomerInsights.WebhookWorker.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<WebhookSender>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

IEnumerable<TimeSpan> retryDelays = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), retryCount: 6);
AsyncRetryPolicy<HttpResponseMessage> policy = Policy<HttpResponseMessage>
   .Handle<HttpRequestException>()
   .OrResult(r => (int)r.StatusCode >= 500 || r.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
   .WaitAndRetryAsync(retryDelays);

builder.Services.AddSingleton(policy);

builder.Services.Configure<RabbitOptions>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddHostedService<RabbitConsumerService>();

IHost host = builder.Build();
host.Run();
