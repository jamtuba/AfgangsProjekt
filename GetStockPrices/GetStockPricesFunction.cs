using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GetStockPrices.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AP.ClassLibrary.Helpers;
using RabbitMQ.Client;
using System.Text;

namespace GetStockPrices
{
    public class GetStockPricesFunction
    {
        [FunctionName("GetStockPricesFunction")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            //[RabbitMQ(ConnectionStringSetting = "CloudAMQPConnectionString")] IModel channel,
            ILogger log)
        {
            string result;

            // Development or Production
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")))
            {
                // Call API
                var client = new HttpClient();
                var newRequest = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("AlphaVantage"));


                // Read API
                var response = await client.SendAsync(newRequest);
                result = await response.Content.ReadAsStringAsync();

            }
            else
            {
                // Local file
                result = File.ReadAllText("C:\\Users\\jamtu\\Dropbox\\Uddannelse\\Afgangsprojekt\\EksamensKode\\AfgangsProjekt\\GetStockPrices\\TestJson.json");
            }

            var rootObject = JsonConvert.DeserializeObject<RootClass>(result);


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
            var exchange = Endpoints.ExchangeName;
            var conString = Environment.GetEnvironmentVariable("CloudAMQPConnectionString");

            var connection = GetConnection.ConnectionGetter(conString);

            using var channel = connection.CreateModel();

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

            var message = JsonConvert.SerializeObject(rootObject);
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