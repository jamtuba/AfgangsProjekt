using GetStockPrices;

namespace AP.FunctionTests
{
    public class GlobalSetup
    {
        public static void ConfigureEnvironmentVariablesFromLocalSettings()
        {
            var path = Path.GetDirectoryName(typeof(GetStockPricesFunction).Assembly.Location);
            var json = File.ReadAllText(Path.Join(path, "local.settings.json"));
            var parsed = Newtonsoft.Json.Linq.JObject.Parse(json).Value<Newtonsoft.Json.Linq.JObject>("Values");

            foreach (var item in parsed)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value.ToString());
            }
        }
    }
}
