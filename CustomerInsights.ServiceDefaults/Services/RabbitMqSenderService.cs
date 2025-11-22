using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public sealed class RabbitMqSenderService
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqSenderService> _logger;

    public RabbitMqSenderService(IConnection connection, ILogger<RabbitMqSenderService> logger)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        _connection = connection;
        _logger = logger;
    }

    public async Task SendMessageAsync(string queueName, string messageContent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("RabbitMqSenderOptions.QueueName is empty.");
        }

        IChannel channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        try
        {
            await channel.QueueDeclareAsync(queueName, true, false, false, null, cancellationToken: cancellationToken);

            byte[] messageBytes = Encoding.UTF8.GetBytes(messageContent);
            ReadOnlyMemory<byte> messageBody = new ReadOnlyMemory<byte>(messageBytes);

            BasicProperties basicProperties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(string.Empty, queueName, false, basicProperties, messageBody, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to RabbitMQ queue '{QueueName}'.", queueName);
            throw;
        }
        finally
        {
            await channel.DisposeAsync();
        }
    }
}