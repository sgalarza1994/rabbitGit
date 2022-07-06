using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace Receive 
{
    class Program
    {
        static void Main(string[]args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    // channel.QueueDeclare(queue:"hello",
                    // durable:false, exclusive:false,
                    // autoDelete:false,
                    // arguments:null
                    // );
                    channel.ExchangeDeclare(exchange:"logs",
                    type:ExchangeType.Fanout
                    
                    );

                    var queueName = channel.QueueDeclare().QueueName;
                    var consumer = new EventingBasicConsumer(
                        channel

                    );
                    channel.QueueBind(queue:queueName,exchange:"logs",
                    routingKey : ""
                    );
                    Console.WriteLine("Waiting for logs...");
                    consumer.Received +=(model,ea) =>{
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine($"[x] recive {message}");
                    };
                    channel.BasicConsume(queue:queueName,autoAck:true
                        ,consumer:consumer            
                    );

                    Console.WriteLine("Press any ket to exit..");
                    Console.ReadLine();
                }
            }


            
        }
    }
}