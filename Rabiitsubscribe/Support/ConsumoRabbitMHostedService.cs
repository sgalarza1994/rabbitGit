using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rabiitsubscribe.Support
{
    public class ConsumoRabbitMHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        public ConsumoRabbitMHostedService(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger<ConsumoRabbitMHostedService>();
            InitRabbitMQ();
        }
        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            _connection = factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare("DemoQueue", false, false, false, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumor = new EventingBasicConsumer(_channel);
            consumor.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumor.Shutdown += OnConsumerShutdown;
            consumor.Registered += OnConsumerRegistered;
            consumor.Unregistered += OnConsumerUnregistered;
            consumor.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("DemoQueue", false, consumor);
            return Task.CompletedTask;
        }


        private void HandleMessage(string content)
        {
            Console.WriteLine("Receive", content);
            _logger.LogInformation($"consumer received{content}");
        }
        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
