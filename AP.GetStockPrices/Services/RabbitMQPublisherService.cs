using AP.ClassLibrary.Helpers;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AP.GetStockPrices.Services;

public class RabbitMQPublisherService : IRabbitMQPublisherService
{
    public (string, byte[]) PublishRabbitMQ(List<CompanyInfo> companies)
    {
        var conString = Environment.GetEnvironmentVariable("CloudAMQPConnectionString");

        var connection = GetConnection.ConnectionGetter(conString);

        using var channel = connection.CreateModel();

        var exchange = Endpoints.ExchangeName;
        var queue = Endpoints.StockFeederQueue;
        var routingKey = Endpoints.StockValueInRoutingKey;

        channel.ExchangeDeclare(exchange: exchange,
                                durable: true,
                                type: ExchangeType.Topic);

        channel.QueueDeclare(queue: queue,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        channel.QueueBind(queue, exchange, routingKey);

        var message = JsonConvert.SerializeObject(companies);
        var body = Encoding.UTF8.GetBytes(message);

        var props = channel.CreateBasicProperties();
        props.Persistent = true;
        props.ContentType = "application/json";

        channel.BasicPublish(exchange: exchange,
                             routingKey: routingKey,
                             basicProperties: props,
                             body: body);

        CloseConnection.CloseAll(channel, connection);

        return (queue, body);
    }
}
