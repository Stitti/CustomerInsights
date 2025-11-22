using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using CustomerInsights.WebhookService.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerInsights.WebhookService;

public class Worker : MessagingWorkerBase
{
    private const string QueueName = "webhooks";
    private readonly WebhookSenderService _webhookSenderService;
    private CancellationToken _cancellationToken;

    public Worker(WebhookSenderService webhookSenderService, ILogger<Worker> logger, IOptions<RabbitMqConnectionOptions> options) : base(QueueName, logger, options)
    {
        _webhookSenderService = webhookSenderService;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        if (_channel == null)
        {
            _logger.LogError("RabbitMQ channel is not initialized.");
            return;
        }

        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += HandleReceivedMessageAsync;

        await _channel.BasicConsumeAsync(QueueName, false, consumer, _cancellationToken);
        return;
    }

    protected override Task HandleReceivedMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        throw new NotImplementedException();
    }
}
