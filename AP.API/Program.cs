global using AP.ClassLibrary.Model;
global using RabbitMQ.Client;
using AP.API.Hubs;
using AP.API.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;


//var AllowedSpecificOrigins = "_allowedSpecificOrigins";

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

builder.Services.AddSignalR()
    .AddAzureSignalR();

// options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString("AzureSignalRConnectionString");
//}

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
builder.Services.AddCors();

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

app.Run();
