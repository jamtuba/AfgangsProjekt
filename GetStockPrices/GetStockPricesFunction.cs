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
using GetStockPrices.Services;
using System.Linq;

namespace GetStockPrices
{
    public class GetStockPricesFunction
    {
        private readonly HttpClient _client = new();
        //private readonly IGetJsonService _getJsonService;
        private readonly IWebScraperService _webScraperService;
        private readonly string nodeId = "137";

        public GetStockPricesFunction(IHttpClientFactory clientFactory, /*IGetJsonService getJsonService, */IWebScraperService webScraperService)
        {
            _client = clientFactory.CreateClient();
            //_getJsonService = getJsonService;
            _webScraperService = webScraperService;
        }

        [FunctionName(nameof(GetStockPrices))]
        public async Task GetStockPrices(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            ILogger log)
        {
            // Webscraper
            var targetString = $"//div[contains(@id, 'node-{nodeId}')]";
            var targetUrl = "https://npinvestor.dk/aktier-og-kurslister/aktier/danmark/alle-danske-aktier";

            HtmlWeb web = new();

            HtmlDocument document = await web.LoadFromWebAsync(targetUrl);

            HtmlNodeCollection targetNodes = document.DocumentNode.SelectNodes(targetString);

            var companies = _webScraperService.GetNodes(targetNodes);
            //foreach (var company in companies)
            //{
            //    log.LogInformation($"CompanyId : {company.CompanyId}\t CompanyName: {company.CompanyName}\t Value: {company.Value}\t Date : {company.Time}");
            //}
            log.LogInformation($"Number of companies: {companies.Count}");
            log.LogInformation($"Value of company 1: {companies.FirstOrDefault().Value}");




            //var rootObject = await _getJsonService.GetJsonFromApi();


            //if (rootObject.TimeSeries != null)
            //{

            //    foreach (var dailyData in rootObject.TimeSeries)
            //    {
            //        log.LogInformation($"Date: {dailyData.Key}");
            //        log.LogInformation($"Open: {dailyData.Value.Open}");
            //        log.LogInformation($"High: {dailyData.Value.High}");
            //        log.LogInformation($"Low: {dailyData.Value.Low}");
            //        log.LogInformation($"Close: {dailyData.Value.Close}");
            //        log.LogInformation($"Adjusted Close: {dailyData.Value.AdjustedClose}");
            //        log.LogInformation($"Volume: {dailyData.Value.Volume}");
            //        log.LogInformation($"Dividend Amount: {dailyData.Value.DividendAmount}");
            //        log.LogInformation($"Split Coefficient: {dailyData.Value.SplitCoefficient}");
            //    }
            //}

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");


            // RabbiMQ
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
        }
    }
}