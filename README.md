# MassTransit RabbitMQ routing example

Pretty simple example of publish and consume to topic exchange with routing key bingings using MassTransit.

## Getting Started

Initialize RabbitMQ
```
docker-compose up -d
```
Run consumers
```
dotnet run -p ./src/Example.Consumers
```
Run producer
```
dotnet run -p ./src/Example.Producer
```