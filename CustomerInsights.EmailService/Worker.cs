using CustomerInsights.EmailService.Services;
using CustomerInsights.Models.Models.Requests;
using CustomerInsights.ServiceDefaults;
using CustomerInsights.ServiceDefaults.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CustomerInsights.EmailService;

public sealed class Worker : MessagingWorkerBase
{
    private const string QueueName = "email_jobs";

    private readonly EmailSendingService _emailSendingService;
    private readonly EmailTemplateProvider _templateProvider;
    private readonly LiquidTemplateRenderer _templateRenderer;
    private CancellationToken _cancellationToken;

    public Worker(ILogger<Worker> logger, IOptions<RabbitMqConnectionOptions> options, EmailSendingService emailService, EmailTemplateProvider templateProvider, LiquidTemplateRenderer templateRenderer) : base(QueueName, logger, options)
    {
        _emailSendingService = emailService;
        _templateProvider = templateProvider;
        _templateRenderer = templateRenderer;
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

    protected override async Task HandleReceivedMessageAsync(object model, BasicDeliverEventArgs eventArgs)
    {
        byte[] body = eventArgs.Body.ToArray();
        string json = Encoding.UTF8.GetString(body);

        try
        {
            EmailJobMessage? message = JsonConvert.DeserializeObject<EmailJobMessage>(json);
            if (message == null)
            {
                _logger.LogError("EmailJobMessage has invalid data");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                return;
            }

            if (message.Recipients == null || message.Recipients.Length == 0)
            {
                _logger.LogError("EmailJobMessage has no recipients");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                return;
            }

            bool success = await ProcessEmailJobAsync(message, _cancellationToken);
            if (success == false)
            {
                _logger.LogError("Failed to send email");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
                return;
            }

            await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, _cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fehler beim Verarbeiten der Email-Message: {Json}", json);
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, _cancellationToken);
        }
    }

    private async Task<bool> ProcessEmailJobAsync(EmailJobMessage job, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verarbeite EmailJob. Template={TemplateKey}, Recipients={Count}", job.TemplateKey, job.Recipients?.Length ?? 0);
        

        string? template = await _templateProvider.GetTemplateAsync(job.TemplateKey, job.LanguageCode);
        if (string.IsNullOrEmpty(template))
        {
            return false;
        }

        string? body = await _templateRenderer.RenderAsync(template, job.Model);
        if (string.IsNullOrEmpty(body))
        {
            return false;
        }

        return await _emailSendingService.SendEmailAsync(job.Subject, body, job.Recipients ?? Array.Empty<string>(), true, cancellationToken);
    }
}
