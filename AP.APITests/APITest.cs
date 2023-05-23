using AP.API.Services;
using RabbitMQ.Client;

namespace AP.APITests;

public class APITest
{
    private readonly RabbitMQService _rabbitMQService;
    private readonly IConnection _connection;
    private readonly string _url;

    public APITest()
    {
        // Arrange
        APITestSetup.ConfigureEnvironmentVariablesFromAppSettings();
        _rabbitMQService = new RabbitMQService();
        _url = Environment.GetEnvironmentVariable("CloudAMQPConnectionString")!;
        _connection = _rabbitMQService.CreateConnection(_url);
    }

    [Fact]
    public void RabbitMQService_Should_Return_IConnection()
    {
        // Arrange

        // Act

        // Assert
        Assert.NotNull(_connection);
        Assert.IsAssignableFrom<IConnection>(_connection);
    }
}