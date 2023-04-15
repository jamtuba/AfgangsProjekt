namespace AP.API.Services;

public interface IRabbitMQConsumer
{
    Task ReadMessages();
}
