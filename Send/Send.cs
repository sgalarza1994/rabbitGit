using System;
using System.Text;
using RabbitMQ.Client;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
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
                    string message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange:"logs",
                    routingKey:"",
                    basicProperties:null,
                    body:body
                    );
                    Console.Write($"[x] Sent {message}");
                }
            }
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static string GetMessage(string [] args)
        {
            return ((args.Length > 0 ? string.Join(" ", args): "ifo:Hola mundo"));
        }
    }
}