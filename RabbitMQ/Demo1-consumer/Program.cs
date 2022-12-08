using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo1_consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(@"amqp://username:password@127.0.0.1:5672/demo_virtual_host"),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                AutomaticRecoveryEnabled = true,
            };

            using var connection = connectionFactory.CreateConnection();

            using var model = connection.CreateModel();

            model.ExchangeDeclare(exchange: "hello_exchange", type: "fanout", durable: true, autoDelete: false, arguments: null);
            model.QueueDeclare(queue: "hello_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            model.QueueBind(queue: "hello_queue", exchange: "hello_exchange", routingKey: string.Empty, arguments: null);

            model.BasicQos(0, 15_000 * 4, false);

            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (innerModel, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                CustomObject customObject;
                try
                {
                    customObject = JsonSerializer.Deserialize<CustomObject>(message);
                }
                catch (Exception)
                {
                    model.BasicReject(ea.DeliveryTag, false);
                    throw;
                }

                try
                {
                    /**/
                    // Call your code
                    new ServiceXPTO().DoAnything(customObject);
                    /**/
                    model.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    model.BasicNack(ea.DeliveryTag, false, true);
                    throw;
                }
                //Console.WriteLine(" [x] Received {0}", message);
            };
            model.BasicConsume(queue: "hello_queue",
                                 autoAck: false,
                                 consumer: consumer);

            Console.ReadLine();


            Console.WriteLine("Hello World!");
        }
    }
}
