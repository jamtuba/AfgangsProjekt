using AP.GetStockPrices;

namespace AP.FunctionTests;

public class GlobalSetup
{
    public static void ConfigureEnvironmentVariablesFromLocalSettings()
    {
        string _fileName = "local.settings.json";

        var path = Path.GetDirectoryName(typeof(GetStockPricesFunction).Assembly.Location);

        if (File.Exists(Path.Join(path, _fileName)))
        {
            var json = File.ReadAllText(Path.Join(path, _fileName));
            var parsed = Newtonsoft.Json.Linq.JObject.Parse(json).Value<Newtonsoft.Json.Linq.JObject>("Values");

            foreach (var item in parsed)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value.ToString());
            }
        }
    }
}
