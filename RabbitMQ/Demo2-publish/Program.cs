using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Demo2_publish.Extras;

namespace Demo2_publish
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(@"amqp://username:password@127.0.0.1:5672/demo_virtual_host"),
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                AutomaticRecoveryEnabled = true
            };

            using var connection = connectionFactory.CreateConnection();

            using var model = connection.CreateModel();

            model.ConfirmSelect();

            model.ExchangeDeclare(exchange: "hello_exchange", type: "fanout", durable: true, autoDelete: false, arguments: null);
            model.QueueDeclare(queue: "hello_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            model.QueueBind(queue: "hello_queue", exchange: "hello_exchange", routingKey: string.Empty, arguments: null);
            //model.ExchangeBind(source: "hello_exchange", destination: "hello_exchange", routingKey: string.Empty, arguments: null);

            var obj = new CustomObject()
            {
                Text = "Hello World! " + DateTime.Now.ToString()
            };

            string message = System.Text.Json.JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(message);

            var props = model.CreateBasicProperties();
            props.Headers = new Dictionary<String, Object> {
                    { "content-type", "application/json" }
                };
            props.DeliveryMode = 2;


            for (var i = 1; i <= 100_000_000; i++)

                model.BasicPublish(exchange: "hello_exchange",
                                     routingKey: "",
                                     basicProperties: props,
                                     body: body);


            Console.WriteLine("Hello World!");
        }
    }
}
