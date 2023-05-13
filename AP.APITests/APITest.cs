using AP.API.Hubs;
using AP.API.Services;
using AP.ClassLibrary.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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
    public void RabbitMQService_Return_Should_IConnection()
    {
        // Arrange

        // Act

        // Assert
        Assert.NotNull(_connection);
        Assert.IsAssignableFrom<IConnection>(_connection);
    }

    //[Fact]
    //public async Task RabbitMQConsumer_Using_RabbitMQService()
    //{

    //    // Arrange
    //    //var consumer = new AsyncEventingBasicConsumer(_model);

    //    //_mockRabbitMQService.Setup(rs => rs.CreateConnection(_url))
    //    //    .Returns(_connection);

    //    var mockClients = new Mock<IHubClients>();
    //    //var mockClientProxy = new Mock<IClientProxy>();
    //    //mockClients.Setup(c => c.All)
    //    //    .Returns(mockClientProxy.Object);

    //    var hubContext = new Mock<IHubContext<SignalRHub>>();
    //    hubContext.Setup(h => h.Clients)
    //        .Returns(() => mockClients.Object);

    //    // Act
    //    var mockrabbitMQConsumer = new Mock<RabbitMQConsumer>(_mockRabbitMQService.Object, hubContext, _configuration);

    //    //mockrabbitMQConsumer.Setup(mrc => mrc.ReadMessages())
    //    //    .Returns(It.IsAny<Task>());

    //    // Assert
    //    //_mockRabbitMQService.VerifyAll();
    //    mockrabbitMQConsumer.VerifyAll();
    //    mockrabbitMQConsumer.Verify(h => h.Dispose());
    //    //mockrabbitMQConsumer.Verify(c => c.ReadMessages());
    //}

    [Fact]
    public async Task SignalRHub_SendsAsync()
    {
        TestServer server = null;
        var message = new List<CompanyInfo>() {
            new CompanyInfo {
                CompanyId = 1,
                CompanyName = "Test",
                Time = DateTime.Now.ToString(),
                Value = "1234"
        } }; ;
        var echo = new List<CompanyInfo>();
        var webHostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSignalR();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(routes => routes.MapHub<SignalRHub>("/thehub"));
            });

        server = new TestServer(webHostBuilder);
        var connection = new HubConnectionBuilder()
            .WithUrl(
            "http://localhost/thehub",
            o => o.HttpMessageHandlerFactory = _ => server.CreateHandler())
            .Build();

        await connection.StartAsync();
        connection.On<List<CompanyInfo>>("SendMessage", msg =>
        {
            echo = msg;
        });

        await connection.InvokeAsync("SendMessage", message);

        var waiter = string.Empty;
        //Assert.Equal(message, echo);
    }

    //[Fact]
    //public async Task ReadMessages_Should_Read_Messages_And_Invoke_SignalR_Client()
    //{
    //    // Arrange
    //    //var modelMock = new Mock<IModel>();
    //    var hubContextMock = new Mock<IHubContext<SignalRHub>>();
    //    var clientsMock = new Mock<IHubClients>();
    //    hubContextMock.Setup(x => x.Clients).Returns(clientsMock.Object);
    //    var consumer = new AsyncEventingBasicConsumer(_model);

    //    byte[] messageBody = Encoding.UTF8.GetBytes("[{\"CompanyName\":\"Company A\",\"Value\":100}, {\"CompanyName\":\"Company B\",\"Value\":200}]");
    //    var eventArgs = new BasicDeliverEventArgs()
    //    {
    //        Body = messageBody
    //    };

    //    var companies = JsonConvert.DeserializeObject<List<CompanyInfo>>(Encoding.UTF8.GetString(messageBody));
    //    var expectedInvocationCount = 1;

    //    clientsMock.Setup(x => x.All.SendAsync("RecieveStockData", companies))
    //        .Returns(Task.CompletedTask)
    //        .Verifiable(expectedInvocationCount);

    //    // Act
    //    await consumer.HandleBasicDeliver("consumerTag", 1, false, "exchange", "routingKey", null,);

    //    // Assert
    //    clientsMock.Verify();
    //}
}