using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace AP.GetStockPrices.Services;

public class GetJsonService : IGetJsonService
{
    private readonly HttpClient _client;

    public GetJsonService(IHttpClientFactory client)
    {
        _client = client.CreateClient();
    }

    public async Task<RootClass> GetJsonFromApi()
    {
        string result;

        //Development or Production
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("IS_RUNTIME_LOCAL")))
        {
            // Call API
            var newRequest = new HttpRequestMessage(HttpMethod.Get, Environment.GetEnvironmentVariable("AlphaVantage"));


            // Read API
            var response = await _client.SendAsync(newRequest);
            result = await response.Content.ReadAsStringAsync();

        }
        else
        {
            // Local file
            var path = Path.GetDirectoryName(typeof(GetStockPricesFunction).Assembly.Location);
            result = File.ReadAllText(Path.Join(path, "TestJson.json"));
        }

        var rootObject = JsonConvert.DeserializeObject<RootClass>(result);

        return rootObject;
    }
}
