using AP.ClassLibrary.Helpers;
using GetStockPrices;
using GetStockPrices.Models;
using GetStockPrices.Services;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Net;
using System.Text;

namespace AP.FunctionTests
{
    public class GetStockPriceTests
    {
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockFactory;
        private readonly Mock<IGetJsonService> _mockGetJson;
        private readonly Mock<IWebScraperService> _mockWebscraper;


        public GetStockPriceTests()
        {
            GlobalSetup.ConfigureEnvironmentVariablesFromLocalSettings();
            _mockLogger = new();
            _mockFactory = new();
            _mockGetJson = new();
            _mockWebscraper = new();


            _mockGetJson.Setup<Task<RootClass>>(gj => gj.GetJsonFromApi())
                           .ReturnsAsync(new RootClass());

            _mockWebscraper.Setup<List<CompanyInfo>>(ws => ws.GetNodes(It.IsAny<HtmlNodeCollection>()))
                .Returns(new List<CompanyInfo>());
        }


        [Fact]
        public async Task Timer_should_log_message()
        {
            // Arrange

            // Act

            var getStockPrices = new GetStockPricesFunction(_mockFactory.Object, _mockGetJson.Object, _mockWebscraper.Object);
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


        // Der skal laves en ny test...

        //[Fact]
        //public async Task Check_That_HttpClient_Is_Called_Exactly_Once()
        //{
        //    // Arrange

        //    _mockGetJson.Setup<Task<RootClass>>(gj => gj.GetJsonFromApi())
        //                   .ReturnsAsync(new RootClass());

        //    _mockWebscraper.Setup<List<CompanyInfo>>(ws => ws.GetNodes(It.IsAny<HtmlNodeCollection>()))
        //        .Returns(new List<CompanyInfo>());

        //    var handler = new Mock<HttpMessageHandler>();

        //    handler.SetupAnyRequest()
        //        .ReturnsResponse(HttpStatusCode.NotFound);


        //    handler
        //        .Protected()
        //        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        //        });


        //    var client = new HttpClient(handler.Object);

        //    _mockFactory
        //        .Setup(x => x.CreateClient(It.IsAny<string>()))
        //        .Returns(client)
        //        .Verifiable();

        //    // Act

        //    var getStockPrices = new GetStockPricesFunction(_mockFactory.Object, _mockGetJson.Object, _mockWebscraper.Object);
        //    await getStockPrices.GetStockPrices(null, _mockLogger.Object);


        //    // Assert
        //    handler
        //        .Protected()
        //        .Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        //}

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
        public async Task GetJson_Return_RootObjectAsync()
        {
            // Arrange
            var expectedJson = File.ReadAllText("C:\\Users\\jamtu\\Dropbox\\Uddannelse\\Afgangsprojekt\\EksamensKode\\AfgangsProjekt\\GetStockPrices\\TestJson.json");
            var expectedRootObject = JsonConvert.DeserializeObject<RootClass>(expectedJson);

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedJson, Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            _mockFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient)
                .Verifiable();

            var sut = new GetJsonService(_mockFactory.Object);


            // Act
            var actualRootObject = await sut.GetJsonFromApi();


            // Assert
            Assert.IsType<RootClass>(actualRootObject);
            Assert.Equal(expectedRootObject.MetaData, actualRootObject.MetaData);
            _mockFactory.Verify();

        }


        [Fact]
        public void GetNodes_Return_List_Of_CompanyInfo()
        {

            // Arrange
            List<CompanyInfo> expectedList = new()
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

            var html = @"<div><div id='node-137'><div><div id='137_company_name_0'><a>Test1</a></div><div id='137_last_0'>111</div></div><div><div id='137_company_name_1'><a>Test2</a></div><div id='137_last_1'>222</div></div><div><div id='137_company_name_2'><a>Test3</a></div><div id='137_last_2'>333</div></div></div></div>";

            var targetString = "//div[contains(@id, 'node-137')]";

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nodes = doc.DocumentNode.SelectNodes(targetString);

            _mockWebscraper.Setup(ws => ws.GetNodes(It.IsAny<HtmlNodeCollection>()))
                .Returns(expectedList);

            var webscarper = new WebScraperService();


            // Act
            var result = webscarper.GetNodes(nodes);


            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(expectedList[0].CompanyName, result[0].CompanyName);
            Assert.Equal(expectedList[2].Value, result[2].Value);
        }
    }
}