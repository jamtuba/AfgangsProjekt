using RabbitMQ.Client;

namespace AP.ClassLibrary.Helpers;

public static class GetConnection
{
    public static IConnection ConnectionGetter(string uri)
    {
        var factory = new ConnectionFactory { Uri = new Uri(uri) };

        var connection = factory.CreateConnection();

        return connection;
    }
}
