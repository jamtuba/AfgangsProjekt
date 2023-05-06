namespace AP.API.Services;

public interface IRabbitMQService
{
    IConnection CreateConnection();
}