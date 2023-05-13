using RabbitMQ.Client;

namespace AP.ClassLibrary.Helpers;

public static class GetConnection
{
    public static IConnection ConnectionGetter(string uri)
    {
        ConnectionFactory factory = new()
        {
            Uri = new Uri(uri),
            DispatchConsumersAsync = true
        };

        var connection = factory.CreateConnection();

        return connection;
    }
}
