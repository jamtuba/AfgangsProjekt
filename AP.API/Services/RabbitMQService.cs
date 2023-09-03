namespace AP.API.Services;

public class RabbitMQService : IRabbitMQService
{
    public IConnection CreateConnection(string url)
    {
        ConnectionFactory connection = new();
        connection.Uri = new Uri(url);
        connection.DispatchConsumersAsync = true; // enabler async consumer dispatcher

        var channel = connection.CreateConnection();

        return channel;
        // Just a comment to run Actions
    }
}
