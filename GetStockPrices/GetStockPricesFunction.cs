using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GetStockPrices.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace GetStockPrices
{
    public class GetStockPricesFunction
    {
        [FunctionName("GetStockPricesFunction")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, 
            ILogger log)
        {
            // Call API
            HttpClient client = new HttpClient();
            HttpRequestMessage newRequest = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("AlphaVantage"));

            // Read API
            HttpResponseMessage response = await client.SendAsync(newRequest);
            var result = await response.Content.ReadFromJsonAsync<StockClass.MetaData>();

            

            foreach (var item in result.GetType().ToString())
            {

                log.LogInformation($"Resultat: {item}");
            }

            log.LogInformation($"Resultat: {result}");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
