global using AP.ClassLibrary.Model;
global using RabbitMQ.Client;
using AP.API.Hubs;

var builder = WebApplication.CreateBuilder(args);


//var AllowedSpecificOrigins = "_allowedSpecificOrigins";

// Giver mulighed for at bruge appsettings variabler her
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//    .AddEnvironmentVariables()
//    .AddUserSecrets<Program>()
//    .AddCommandLine(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

builder.Services.AddSignalR();


//builder.Services.AddResponseCompression(opts =>
//{
//    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//        new[] { "application/octet-stream" });
//});


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: AllowedSpecificOrigins,
//        policy =>
//        {
//            policy.WithOrigins("https://localhost:7132", "https://ambitious-field-0972b7003.3.azurestaticapps.net");
//        });
//});

//builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
//builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
//builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();


// Nødvendig for at kunne oprette en SignalR forbindelse
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

//var webSocketOptions = new WebSocketOptions
//{
//    KeepAliveInterval = TimeSpan.FromMinutes(2)
//};

//webSocketOptions.AllowedOrigins.Add("https://localhost:7132");
//webSocketOptions.AllowedOrigins.Add("https://ambitious-field-0972b7003.3.azurestaticapps.net");

app.MapHub<SignalRHub>(SignalRHub.HubUrl);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}