using CustomerInsights.EmbeddingService.Repositories;
using CustomerInsights.EmbeddingService.Services;
using CustomerInsights.Models;
using CustomerInsights.Models.Models.Requests;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerInsights.EmbeddingService
{
    public sealed class Worker : MessagingWorkerBase
    {
        private const string QueueName = "embedding_jobs";
        private readonly OnnxEmbeddingProvider _onnxEmbeddingProvider;
        private readonly InteractionRepository _interactionRepository;
        private readonly InteractionEmbeddingRepository _interactionEmbeddingRepository;
        private CancellationToken _cancellationToken;

        public Worker(OnnxEmbeddingProvider  onnxEmbeddingProvider, InteractionRepository interactionRepository,  InteractionEmbeddingRepository interactionEmbeddingRepository, ILogger<Worker> logger, IOptions<RabbitMqConnectionOptions> options) : base(QueueName, logger, options)
        {
            _onnxEmbeddingProvider = onnxEmbeddingProvider;
            _interactionRepository = interactionRepository;
            _interactionEmbeddingRepository = interactionEmbeddingRepository;
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
                EmbeddingJobMessage? message = JsonConvert.DeserializeObject<EmbeddingJobMessage>(json);
                if (message == null)
                {
                    _logger.LogError("EmbeddingJobMessage has invalid data");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }

                if (message.InteractionId == Guid.Empty)
                {
                    _logger.LogError("EmbeddingJobMessage has an invalid interaction id");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }

                Interaction? interaction = await _interactionRepository.GetInteractionByIdAsync(message.InteractionId, message.TenantId, _cancellationToken);
                if (interaction == null) 
                {
                    _logger.LogError("Interaction with id {InteractionId} on tenant {TenantId} not found", message.InteractionId, message.TenantId);
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }

                bool exists = await _interactionEmbeddingRepository.CheckForExistingEmbeddingAsync(interaction.Id, message.TenantId, _cancellationToken);
                if (exists)
                {
                    _logger.LogInformation("Embedding for interaction {InteractionId} on tenant {TenantId} already exists", message.InteractionId, message.TenantId);
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, _cancellationToken);
                    return;

                }

                float[] embeddingVector = await _onnxEmbeddingProvider.CreateEmbeddingAsync(interaction.Text);
                bool success = await _interactionEmbeddingRepository.CreateInteractionEmbeddingAsync(interaction, embeddingVector, _cancellationToken);
                if (!success)
                {
                    _logger.LogError("Failed to create embedding for interaction {InteractionId} on tenant {TenantId}", message.InteractionId, message.TenantId);
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }

                _logger.LogInformation("Successfully created embedding for interaction {InteractionId} on tenant {TenantId}", message.InteractionId, message.TenantId);
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, _cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing the embedding message: {Json}", json);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
            }
        }
    }
}
