using System;
using Example.Contracts;
using MassTransit;
using RabbitMQ.Client;

namespace Example.Producer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost:5673/vhost"), x =>
                {
                    x.Username("admin");
                    x.Password("mypass");
                });

                // configure send topology with country routing key and topic exchange
                cfg.Send<TestMessage>(x => { x.UseRoutingKeyFormatter(context => context.Message.Country); });
                cfg.Publish<TestMessage>(x => { x.ExchangeType = ExchangeType.Topic; });
            });

            bus.Start();

            Console.WriteLine("Bus started. Consume messages via Example.Consumers");
            while (true)
            {
                Console.Write("message: ");
                var message = Console.ReadLine();

                Console.Write("country: ");
                var country = Console.ReadLine();

                bus.Publish(new TestMessage
                {
                    Country = country,
                    Message = message
                });
            }
        }
    }
}

namespace Example.Contracts
{
    public class TestMessage
    {
        public string Country { get; set; }
        public string Message { get; set; }
    }
}