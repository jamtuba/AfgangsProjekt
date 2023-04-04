using RabbitMQ.Client;

namespace AP.ClassLibrary.Helpers;

public static class CloseConnection
{
    public static void CloseAll(IModel channel, IConnection connection)
    {
        channel.Close();
        connection.Close();
    }
}