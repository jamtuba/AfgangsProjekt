using AP.API.Services;
using Microsoft.Extensions.Configuration;

namespace AP.APITests;

public class APITestSetup
{
    static readonly string _fileName = "appsettings.json";
    public static void ConfigureEnvironmentVariablesFromAppSettings()
    {

        var path = Path.GetDirectoryName(typeof(RabbitMQConsumer).Assembly.Location);

        if (File.Exists(Path.Join(path, _fileName)))
        {
            var json = File.ReadAllText(Path.Join(path, _fileName));

            var parsed = Newtonsoft.Json.Linq.JObject.Parse(json).Value<Newtonsoft.Json.Linq.JObject>("ConnectionStrings");

            foreach (var item in parsed!)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value!.ToString());
            }
        }
    }

    public static IConfigurationRoot GetIConfigurationRoot()
    {
        var path = Path.GetDirectoryName(typeof(RabbitMQConsumer).Assembly.Location);
        var newConf = new ConfigurationBuilder();

        if (File.Exists(Path.Join(path, _fileName)))
        {
            var pathToJson = Path.Join(path, _fileName);
            
            newConf.AddJsonFile(pathToJson)
                    .AddEnvironmentVariables()
                    .Build(); 
        }
        return newConf.Build();
    }
}
