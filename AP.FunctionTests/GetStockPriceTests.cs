using AP.ClassLibrary.Helpers;
using AP.GetStockPrices.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace AP.FunctionTests
{
    public class GetStockPriceTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockFactory;
        private readonly Mock<IWebScraperService> _mockWebscraper;
        private readonly Mock<IRabbitMQPublisherService> _mockRabbitMQPublisher;
        private readonly List<CompanyInfo> _companyList = new()
            { new CompanyInfo
                {
                    CompanyId = 1,
                    CompanyName = "Test1",
                    Value = "111"
                }, new CompanyInfo
                {
                    CompanyId = 2,
                    CompanyName = "Test2",
                    Value = "222"
                }, new CompanyInfo
                {
                    CompanyId = 3,
                    CompanyName = "Test3",
                    Value = "333"
                }
            };

        public GetStockPriceTests()
        {
            GlobalSetup.ConfigureEnvironmentVariablesFromLocalSettings();
            _mockLogger = new();
            _mockFactory = new();
            _mockWebscraper = new();
            _mockRabbitMQPublisher = new();
            _mockWebscraper.Setup<List<CompanyInfo>>(ws => ws.GetNodes(It.IsAny<HtmlNodeCollection>()))
                .Returns(new List<CompanyInfo>());
        }


        [Fact]
        public async Task Timer_should_log_message()
        {
            // Arrange

            // Act
            var getStockPrices = new GetStockPricesFunction(_mockFactory.Object,_mockWebscraper.Object, _mockRabbitMQPublisher.Object);
            await getStockPrices.GetStockPrices(null, _mockLogger.Object);


            // Assert
            _mockLogger.Verify(
        m => m.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("C# Timer trigger function executed at")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);


        }

        [Fact]
        public void RabbitMQPusblisherService_Should_Return_String_And_ByteArray_Tuplet()
        {
            // Arrange
            var sut = new RabbitMQPublisherService();

            _mockRabbitMQPublisher.Setup(r => r.PublishRabbitMQ(_companyList))
                .Returns((It.IsAny<string>(), It.IsAny<byte[]>()));

            // Act
            var result = sut.PublishRabbitMQ(_companyList);
            var expected = _mockRabbitMQPublisher.Object.PublishRabbitMQ(_companyList);

            // Assert
            Assert.Equal(expected.GetType(), result.GetType());
            Assert.IsType<(string, byte[])>(result);
        }

        [Fact]
        public void ConnectionGetter_Should_Return_AmqpTcp_Endpoint()
        {
            // Arrange
            var uri = Environment.GetEnvironmentVariable("CloudAMQPConnectionString");


            // Act
            var sut = GetConnection.ConnectionGetter(uri!);


            // Assert
            Assert.IsType<AmqpTcpEndpoint>(sut.Endpoint);
        }


        [Fact]
        public void GetNodes_Return_List_Of_CompanyInfo()
        {
            // Arrange
            var nodeId = "137";

            var html = $@"<div><div id='node-{nodeId}'><div><div id='{nodeId}_company_name_0'><a>Test1</a></div><div id='{nodeId}_last_0'>111</div></div><div><div id='{nodeId}_company_name_1'><a>Test2</a></div><div id='{nodeId}_last_1'>222</div></div><div><div id='{nodeId}_company_name_2'><a>Test3</a></div><div id='{nodeId}_last_2'>333</div></div></div></div>";

            var targetString = $"//div[contains(@id, 'node-{nodeId}')]";

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(targetString);

            _mockWebscraper.Setup(ws => ws.GetNodes(It.IsAny<HtmlNodeCollection>()))
                .Returns(_companyList);

            var webscraper = new WebScraperService();


            // Act
            var result = webscraper.GetNodes(nodes);


            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(_companyList[0].CompanyName, result[0].CompanyName);
            Assert.Equal(_companyList[2].Value, result[2].Value);
        }
    }
}