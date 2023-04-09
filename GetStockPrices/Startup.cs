global using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using GetStockPrices.Services;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(GetStockPrices.Startup))]

namespace GetStockPrices;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IGetJsonService, GetJsonService>();
        builder.Services.AddScoped<IWebScraperService, WebScraperService>();
    }
}
