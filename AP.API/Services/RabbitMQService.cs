namespace AP.API.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly IConfiguration _configuration;

    public RabbitMQService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConnection CreateConnection()
    {
        var conString = _configuration.GetConnectionString("CloudAMQPConnectionString");
        ConnectionFactory connection = new();
        connection.Uri = new Uri(conString);
        connection.DispatchConsumersAsync = true;

        var channel = connection.CreateConnection();

        return channel;
    }
}
