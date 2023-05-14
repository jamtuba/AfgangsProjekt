using AP.API.Hubs;
using AP.ClassLibrary.Helpers;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;

namespace AP.API.Services;

public class RabbitMQConsumer : IRabbitMQConsumer, IDisposable
{
    private readonly IModel _model;
    private readonly IConnection _connection;
    private readonly IHubContext<SignalRHub> _hubContext;
    private readonly string _exchange = Endpoints.ExchangeName;
    private readonly string _queueName = Endpoints.StockFeederQueue;
    private readonly string _routingKey = Endpoints.StockValueInRoutingKey;
    private readonly IConfiguration _configuration;


    public RabbitMQConsumer(IRabbitMQService rabbitMQService, IHubContext<SignalRHub> hubContext, IConfiguration configuration)
    {
        _configuration = configuration;
        var conString = _configuration.GetConnectionString("CloudAMQPConnectionString");
        _connection = rabbitMQService.CreateConnection(conString);
        _model = _connection.CreateModel();

        _model.ExchangeDeclare(exchange: _exchange,
                                durable: true,
                                type: ExchangeType.Topic);

        _model.QueueDeclare(queue: _queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        _model.QueueBind(_queueName, _exchange, _routingKey);

        _hubContext = hubContext;
    }

    public async Task ReadMessages()
    {
        var consumer = new AsyncEventingBasicConsumer(_model);

        consumer.Received += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var companies = JsonConvert.DeserializeObject<List<CompanyInfo>>(message) ?? new List<CompanyInfo>();
            await _hubContext.Clients.All.SendAsync("RecieveStockData", companies);
            await Task.CompletedTask;
            if (body is not null)
            {
                Console.WriteLine("New object recieved!");
                Console.WriteLine($"Company 1 name: {companies[0].CompanyName}, Value: {companies[0].Value}");
                Console.WriteLine($"Company 48 name: {companies[47].CompanyName}, Value: {companies[47].Value}");
            }
        };

        _model.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_model.IsOpen)
            _model.Close();
        if (_connection.IsOpen)
            _connection.Close();
    }

}
