using Newtonsoft.Json;
using System.Collections.Generic;

namespace AP.GetStockPrices.Models;

public class RootClass
{
    [JsonProperty(PropertyName = "Meta Data")]
    public Dictionary<string, string> MetaData { get; set; }

    [JsonProperty(PropertyName = "Time Series (Daily)")]
    public Dictionary<string, TimeSeriesDaily> TimeSeries { get; set; }

}
public class MetaData
{
    [JsonProperty("1. Information")]
    public string Information { get; set; }

    [JsonProperty("2. Symbol")]
    public string Symbol { get; set; }

    [JsonProperty("3. LastRefreshed")]
    public string LastRefreshed { get; set; }

    [JsonProperty("4. OutputSize")]
    public string OutputSize { get; set; }

    [JsonProperty("5. TimeZone")]
    public string TimeZone { get; set; }
}

public class TimeSeriesDaily
{
    [JsonProperty("1. open")]
    public string Open { get; set; }

    [JsonProperty("2. high")]
    public string High { get; set; }

    [JsonProperty("3. low")]
    public string Low { get; set; }

    [JsonProperty("4. close")]
    public string Close { get; set; }

    [JsonProperty("5. adjusted close")]
    public string AdjustedClose { get; set; }

    [JsonProperty("6. volume")]
    public string Volume { get; set; }

    [JsonProperty("7. dividend amount")]
    public string DividendAmount { get; set; }

    [JsonProperty("8. split coefficient")]
    public string SplitCoefficient { get; set; }
}