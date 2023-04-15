global using AP.ClassLibrary.Model;
global using RabbitMQ.Client;
using AP.API.Hubs;
using AP.API.Services;
using AP.ClassLibrary.Helpers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System.Text;


var AllowedSpecificOrigins = "_allowedSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);


// Giver mulighed for at bruge appsettings variabler her
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .AddCommandLine(args);


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: AllowedSpecificOrigins,
//        policy =>
//        {
//            policy.WithOrigins("https://localhost:7132", "https://ambitious-field-0972b7003.3.azurestaticapps.net");
//        });
//});

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// Nødvendig for at kunne oprette en SignalR forbindelse
app.UseCors(builder => builder
    //.WithOrigins("https://localhost:7132", "https://ambitious-field-0972b7003.3.azurestaticapps.net")
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

app.MapHub<SignalRHub>("/thehub");

//app.MapGet("/", async (IConfiguration config, IHubContext<SignalRHub> hub) =>
//{
//    var newRabbit = new RabbitMQService(config, hub);
//    return await newRabbit.GetRabbitMQ();
//});

//app.MapGet("/", async (IHubContext<SignalRHub> _hubContext) =>
//{
//    List<CompanyInfo> newCompanies = new();
//    newCompanies.Add(new CompanyInfo { CompanyId = 1, CompanyName = "Test", Value = "88.77" });
//    await _hubContext.Clients.All.SendAsync("newStockData", newCompanies);
//    return newCompanies;
//});

//app.MapGet("/", (IConfiguration _configuration, IHubContext<SignalRHub> _hubContext) =>
//{
//    var companies = new List<CompanyInfo>();

//    var conString = _configuration.GetConnectionString("CloudAMQPConnectionString");

//    var connection = GetConnection.ConnectionGetter(conString!);

//    using var channel = connection.CreateModel();

//    var exchange = Endpoints.ExchangeName;
//    var queueName = Endpoints.StockFeederQueue;
//    var routingKey = Endpoints.StockValueInRoutingKey;

//    channel.ExchangeDeclare(exchange: exchange,
//                            durable: true,
//                            type: ExchangeType.Direct);

//    channel.QueueDeclare(queue: queueName,
//                         durable: true,
//                         exclusive: false,
//                         autoDelete: false,
//                         arguments: null);

//    channel.QueueBind(queueName, exchange, routingKey);

//    var consumer = new EventingBasicConsumer(channel);
//    consumer.Received += async (model, ea) =>
//    {
//        byte[] body = ea.Body.ToArray();
//        var message = Encoding.UTF8.GetString(body);
//        companies = JsonConvert.DeserializeObject<List<CompanyInfo>>(message);
//        await _hubContext.Clients.All.SendAsync("NewStockData", companies);
//        Console.WriteLine("New object recieved!");
//        //Message = $" [x] {message}";
//    };

//    channel.BasicConsume(queue: queueName,
//                    autoAck: true,
//                    consumer: consumer);

//    return companies;
//});


app.Run();
