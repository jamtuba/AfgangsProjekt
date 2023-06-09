﻿global using Microsoft.Azure.Functions.Extensions.DependencyInjection;
global using AP.ClassLibrary.Model;
global using AP.GetStockPrices.Services;
global using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AP.GetStockPrices.Startup))]

namespace AP.GetStockPrices;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<IWebScraperService, WebScraperService>();
        builder.Services.AddScoped<IRabbitMQPublisherService, RabbitMQPublisherService>();
    }
}
