using CustomerInsights.ServiceDefaults.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;

namespace CustomerInsights.ServiceDefaults
{
    public abstract class MessagingWorkerBase : BackgroundService
    {
        protected readonly ILogger<MessagingWorkerBase> _logger;
        protected readonly RabbitMqConnectionOptions _options;
        protected IConnection? _connection;
        protected IChannel _channel;

        public MessagingWorkerBase(string queueName, ILogger<MessagingWorkerBase> logger, IOptions<RabbitMqConnectionOptions> options)
        {
            _options = options.Value;
            _logger = logger;

            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password
            };

            _connection = factory.CreateConnectionAsync().Result;
            _channel = _connection.CreateChannelAsync().Result;

            _channel.QueueDeclareAsync(queueName, true, false, false).Wait();
            _channel.BasicQosAsync(0, 1, false).Wait();
            _logger.LogInformation("RabbitMQ EmailWorker connected. Queue: {Queue}", queueName);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        protected override abstract Task ExecuteAsync(CancellationToken cancellationToken);
        protected abstract Task HandleReceivedMessageAsync(object sender, BasicDeliverEventArgs eventArgs);
    }  
}
