using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AP.ClassLibrary.Helpers;
using RabbitMQ.Client;
using System.Text;
using HtmlAgilityPack;
using AP.GetStockPrices.Services;

namespace AP.GetStockPrices
{
    public class GetStockPricesFunction
    {
        private readonly HttpClient _client = new();
        private readonly IWebScraperService _webScraperService;
        private readonly string nodeId = "137";

        public GetStockPricesFunction(IHttpClientFactory clientFactory, /*IGetJsonService getJsonService, */IWebScraperService webScraperService)
        {
            _client = clientFactory.CreateClient();
            //_getJsonService = getJsonService;     THIS SHOULD BE REMOVED!
            _webScraperService = webScraperService;
        }

        [FunctionName(nameof(GetStockPrices))]
        public async Task GetStockPrices(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            // Webscraper
            var targetString = $"//div[contains(@id, 'node-{nodeId}')]";
            var targetUrl = "https://npinvestor.dk/aktier-og-kurslister/aktier/danmark/alle-danske-aktier";

            HtmlWeb web = new();

            web.UsingCache = false;
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0)";


            HtmlDocument document = await web.LoadFromWebAsync(targetUrl);

            HtmlNodeCollection targetNodes = document.DocumentNode.SelectNodes(targetString);

            var companies = _webScraperService.GetNodes(targetNodes);

            log.LogInformation($"Webscraper was executed at {DateTime.Now:G} getting {companies.Count} companies.");



            // RabbitMQ
            var conString = Environment.GetEnvironmentVariable("CloudAMQPConnectionString");

            var connection = GetConnection.ConnectionGetter(conString);

            using var channel = connection.CreateModel();

            var exchange = Endpoints.ExchangeName;
            var queue = Endpoints.StockFeederQueue;
            var routingKey = Endpoints.StockValueInRoutingKey;

            channel.ExchangeDeclare(exchange: exchange,
                                    durable: true,
                                    type: ExchangeType.Direct);

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue, exchange, routingKey);

            var message = JsonConvert.SerializeObject(companies);
            var body = Encoding.UTF8.GetBytes(message);

            var props = channel.CreateBasicProperties();
            //props.CorrelationId = customer.CustomerId;
            props.Persistent = true;

            channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: props,
                                 body: body);

            CloseConnection.CloseAll(channel, connection);
            log.LogInformation($"RabbitMQ has sent a message with {((body.Length <= 1024) ? body.Length + " bytes" : body.Length / 1024 + " kb")} size at {DateTime.Now:G} to CloudAMQP queue {queue}");
        }
    }
}