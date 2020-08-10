using System;
using System.Threading.Tasks;
using Example.Contracts;
using MassTransit;
using RabbitMQ.Client;

namespace Example.Consumers
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost:5673/vhost"), x =>
                {
                    x.Username("admin");
                    x.Password("mypass");
                });

                // configure country specific consumer
                cfg.ReceiveEndpoint(host, "legacy_ru", e =>
                {
                    e.BindMessageExchanges = false;
                    e.Bind<TestMessage>(x =>
                    {
                        x.ExchangeType = ExchangeType.Topic;
                        x.RoutingKey = "ru";
                    });

                    e.Consumer(() => new TestGenericConsumer("RU legacy service"));
                });

                // configure country specific consumer
                cfg.ReceiveEndpoint(host, "legacy_by", e =>
                {
                    e.BindMessageExchanges = false;
                    e.Bind<TestMessage>(x =>
                    {
                        x.ExchangeType = ExchangeType.Topic;
                        x.RoutingKey = "by";
                    });

                    e.Consumer(() => new TestGenericConsumer("BY legacy service"));
                });

                // configure country agnostic consumer
                cfg.ReceiveEndpoint(host, "country_agnostic", e =>
                {
                    e.BindMessageExchanges = false;
                    e.Bind<TestMessage>(x =>
                    {
                        x.ExchangeType = ExchangeType.Topic;
                        x.RoutingKey = "#";
                    });

                    e.Consumer(() => new TestGenericConsumer("Country agnostic service"));
                });
            });

            bus.Start();

            Console.WriteLine("Consumers started. Publish messages via Example.Producer");
            Console.ReadLine();
        }
    }

    public class TestGenericConsumer : IConsumer<TestMessage>
    {
        private readonly string _name;

        public TestGenericConsumer(string name)
        {
            _name = name;
        }

        public Task Consume(ConsumeContext<TestMessage> context)
        {
            Console.WriteLine($"{_name} received message \"{context.Message.Message}\"");
            return Task.CompletedTask;
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