using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;

namespace AP.GetStockPrices
{
    public class GetStockPricesFunction
    {
        private readonly HttpClient _client = new();
        private readonly IWebScraperService _webScraperService;
        private readonly IRabbitMQPublisherService _rabbitMQPublisherService;
        private readonly string _nodeId = "137";

        public GetStockPricesFunction(IHttpClientFactory clientFactory, IWebScraperService webScraperService, IRabbitMQPublisherService rabbitMQPublisherService)
        {
            _client = clientFactory.CreateClient();
            _webScraperService = webScraperService;
            _rabbitMQPublisherService = rabbitMQPublisherService;
        }

        [FunctionName(nameof(GetStockPrices))]
        public async Task GetStockPrices(
            [TimerTrigger("0 */1 * * * *")] TimerInfo _,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            // Webscraper
            var targetString = $"//div[contains(@id, 'node-{_nodeId}')]";
            var targetUrl = "https://npinvestor.dk/aktier-og-kurslister/aktier/danmark/alle-danske-aktier";

            HtmlWeb web = new();

            HtmlDocument document = await web.LoadFromWebAsync(targetUrl);

            HtmlNodeCollection targetNodes = document.DocumentNode.SelectNodes(targetString);

            var companies = _webScraperService.GetNodes(targetNodes);

            log.LogInformation($"Webscraper was executed at {DateTime.Now:G} getting {companies.Count} companies.");


            // RabbitMQ
            var (queue, body) = _rabbitMQPublisherService.PublishRabbitMQ(companies);

            log.LogInformation($"RabbitMQ has sent a message with {((body.Length <= 1024) ? body.Length + " bytes" : body.Length / 1024 + " kb")} size at {DateTime.Now:G} to CloudAMQP queue {queue}");
        }
    }
}