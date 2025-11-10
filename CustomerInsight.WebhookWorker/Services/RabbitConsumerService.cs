using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CustomerInsights.WebhookWorker.Services;

public class RabbitConsumerService : BackgroundService
{
    private readonly ILogger<RabbitConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitOptions _options;

    private IConnection? _conn;
    private IChannel? _channel;

    public RabbitConsumerService(ILogger<RabbitConsumerService> logger, IServiceProvider serviceProvider, IOptions<RabbitOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.User,
            Password = _options.Password,
        };

        _conn = await factory.CreateConnectionAsync();
        _channel = await _conn.CreateChannelAsync();
        await _channel.BasicQosAsync(0, _options.Prefetch, false);
        await _channel.QueueDeclareAsync(queue: _options.QueueName, durable: true, exclusive: false, autoDelete: false);

        AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessage;

        await _channel.BasicConsumeAsync(queue: _options.QueueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("RabbitMQ consumer started on {Queue}", _options.QueueName);
        await Task.CompletedTask;
    }

    private async Task OnMessage(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            string json = Encoding.UTF8.GetString(ea.Body.ToArray());
            IncomingEvent ev = JsonSerializer.Deserialize<IncomingEvent>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (ev == null)
                throw new InvalidOperationException("Invalid event JSON");

            using IServiceScope scope = _serviceProvider.CreateScope();
            WebhookSender senderSvc = scope.ServiceProvider.GetRequiredService<WebhookSender>();

            await senderSvc.DispatchAsync(ev, CancellationToken.None);

            await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public override void Dispose()
    {
        try
        {
            _channel?.CloseAsync().Wait();
            _conn?.CloseAsync().Wait();
        }
        catch
        {
            /* ignore */
        }
        base.Dispose();
    }
}