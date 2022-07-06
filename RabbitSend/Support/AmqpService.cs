using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitSend.Support
{

    public class AmqpService
    {
        private readonly AmqpInfo amqpInfo;
        private readonly ConnectionFactory connectionFactory;
        private const string queueName = "DemoQueue";

        public AmqpService(IOptions<AmqpInfo> options)
        {
            amqpInfo = options.Value;

            connectionFactory = new ConnectionFactory
            {
                UserName = amqpInfo.UserName,
                Password = amqpInfo.Password,
                VirtualHost = amqpInfo.VirtualHost, 
                HostName = amqpInfo.HostName
            };

        }
        public void PublishMessage(object mesage)
        {
            using (var coon = connectionFactory.CreateConnection())
            {
                using (var channel = coon.CreateModel())
                {
                    channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var jsonPayload = JsonConvert.SerializeObject(mesage);
                    var body = Encoding.UTF8.GetBytes(jsonPayload);

                    channel.BasicPublish(exchange: "",
                        routingKey: queueName, basicProperties: null, body: body);
                }
            }
        }

    }
}
