global using Microsoft.Azure.Functions.Extensions.DependencyInjection;
global using AP.ClassLibrary.Model;
using AP.GetStockPrices.Services;
using Microsoft.Extensions.DependencyInjection;
using AP.GetStockPrices.Services;

[assembly: FunctionsStartup(typeof(AP.GetStockPrices.Startup))]

namespace AP.GetStockPrices;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IGetJsonService, GetJsonService>();
        builder.Services.AddScoped<IWebScraperService, WebScraperService>();
    }
}
