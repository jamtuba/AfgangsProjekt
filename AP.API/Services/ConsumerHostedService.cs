namespace AP.API.Services;

public class ConsumerHostedService : BackgroundService
{
    private readonly IRabbitMQConsumer _rabbitMQConsumer;

    public ConsumerHostedService(IRabbitMQConsumer rabbitMQConsumer)
    {
        _rabbitMQConsumer = rabbitMQConsumer;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _rabbitMQConsumer.ReadMessages();
    }
}
