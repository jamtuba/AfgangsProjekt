using GetStockPrices.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace GetStockPrices.Services;

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
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")))
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
            result = File.ReadAllText("C:\\Users\\jamtu\\Dropbox\\Uddannelse\\Afgangsprojekt\\EksamensKode\\AfgangsProjekt\\GetStockPrices\\TestJson.json");
        }

        var rootObject = JsonConvert.DeserializeObject<RootClass>(result);

        return rootObject;
    }
}
