using CustomerInsights.Base.Models.Responses;
using CustomerInsights.NlpService.Contracts;
using CustomerInsights.NlpService.Runtime;
using CustomerInsights.NlpService.Services;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerInsights.NlpService
{
    public sealed class Worker : MessagingWorkerBase
    {
        private const string QueueName = "nlp_jobs";
        private readonly PresidioService _presidioService;
        private readonly IdentityResolvingService _identityResolvingService;
        private readonly TextAnalyzer _textAnalyzer;
        private readonly TextInferenceService _textInferenceService;
        private CancellationToken _cancellationToken;

        public Worker(PresidioService presidioService, IdentityResolvingService identityResolvingService, TextAnalyzer textAnalyzer, TextInferenceService textInferenceService, IOptions<RabbitMqConnectionOptions> options, ILogger<Worker> logger) : base(QueueName, logger, options)
        {
            _presidioService = presidioService;
            _textAnalyzer = textAnalyzer;
            _textInferenceService = textInferenceService;
            _identityResolvingService = identityResolvingService;
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
                NlpJobMessage? message = JsonConvert.DeserializeObject<NlpJobMessage>(json);
                if (message == null)
                {
                    _logger.LogError("NlpJobMessage has invalid data");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }

                if (message.Id == Guid.Empty)
                {
                    _logger.LogError("NlpJobMessage has an invalid interaction id");
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                    return;
                }
                string? text = TextCompressor.Compress(message.Message);
                if (string.IsNullOrEmpty(text))
                {
                    _logger.LogError("NlpJobMessage has an empty message after compression");
                    await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, _cancellationToken);
                    return;
                }
                PresidioAnalyzeAndAnonymizeResult result = await _presidioService.AnalyzeAndAnonymizeAsync(text, _cancellationToken);
                await _identityResolvingService.ResolveAndSetContactAndAccountAsync(message.Id, result.Entities, _cancellationToken);

                NlpResponse response = _textAnalyzer.Analyze(result.AnonymizedText);
                bool success = await _textInferenceService.CreateTextInferenceAsync(message.Id, message.TenantId, response, _cancellationToken);

                if (success == false)
                {
                    _logger.LogError("Failed to create text inference for interaction {InteractionId} for tenant {TenantId}", message.Id, message.TenantId);
                    await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, _cancellationToken);
                    return;
                }

                await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing the email message: {Json}", json);
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
            }
        }
    }
}
