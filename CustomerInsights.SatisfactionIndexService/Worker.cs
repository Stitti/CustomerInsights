using CustomerInsights.Base.Models.Responses;
using CustomerInsights.Models;
using CustomerInsights.NlpService.Contracts;
using CustomerInsights.SatisfactionIndexService.Models;
using CustomerInsights.SatisfactionIndexService.Repositories;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace CustomerInsights.SatisfactionIndexService;

public sealed class Worker : MessagingWorkerBase
{
    private const string QueueName = "satisfaction-index";
    private readonly SatisfactionIndexRecalculationService _satisfactionIndexRecalculationService;
    private readonly InteractionRepository _interactionRepository;
    private CancellationToken _cancellationToken;

    public Worker(SatisfactionIndexRecalculationService satisfactionIndexRecalculationService, InteractionRepository interactionRepository, ILogger<Worker> logger, IOptions<RabbitMqConnectionOptions> options) : base(QueueName, logger, options)
    {
        _satisfactionIndexRecalculationService = satisfactionIndexRecalculationService;
        _interactionRepository = interactionRepository;
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

    protected override async Task HandleReceivedMessageAsync(object sender, BasicDeliverEventArgs eventArgs)
    {
        byte[] body = eventArgs.Body.ToArray();
        string json = Encoding.UTF8.GetString(body);

        try
        {
            SatisfactionIndexJobMessage? message = JsonConvert.DeserializeObject<SatisfactionIndexJobMessage>(json);
            if (message == null || message.InteractionId == Guid.Empty || message.TenantId == Guid.Empty)
            {
                _logger.LogWarning("Received invalid satisfaction index job message: {Json}", json);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                return;
            }

            Interaction? interaction = await _interactionRepository.GetInteractionByIdAsync(message.InteractionId, _cancellationToken);
            if (interaction == null)
            {
                _logger.LogError("Interaction {InteractionId} not found for satisfaction index recalculation for tenant {TenantId}", message.InteractionId, message.TenantId);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                return;
            }

            await _satisfactionIndexRecalculationService.UpdateForAnalyzedInteractionAsync(message.TenantId, interaction, _cancellationToken);
            await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while processing the email message: {Json}", json);
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
        }
    }
}
