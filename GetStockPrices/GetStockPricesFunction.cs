using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GetStockPrices.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GetStockPrices
{
    public class GetStockPricesFunction
    {
        [FunctionName("GetStockPricesFunction")]
        public async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            ILogger log)
        {
            // Call API
            var client = new HttpClient();
            var newRequest = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("AlphaVantage"));


            // Read API
            var response = await client.SendAsync(newRequest);
            var result = await response.Content.ReadAsStringAsync();
            var rootObject = JsonConvert.DeserializeObject<RootClass>(result);


            if (rootObject.TimeSeries != null)
            {

                foreach (var dailyData in rootObject.TimeSeries)
                {
                    Console.WriteLine($"Date: {dailyData.Key}");
                    Console.WriteLine($"Open: {dailyData.Value.Open}");
                    Console.WriteLine($"High: {dailyData.Value.High}");
                    Console.WriteLine($"Low: {dailyData.Value.Low}");
                    Console.WriteLine($"Close: {dailyData.Value.Close}");
                    Console.WriteLine($"Adjusted Close: {dailyData.Value.AdjustedClose}");
                    Console.WriteLine($"Volume: {dailyData.Value.Volume}");
                    Console.WriteLine($"Dividend Amount: {dailyData.Value.DividendAmount}");
                    Console.WriteLine($"Split Coefficient: {dailyData.Value.SplitCoefficient}");
                }
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}