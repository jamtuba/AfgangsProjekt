namespace AP.ClassLibrary.Helpers;

public static class Endpoints
{
    public static string ExchangeName { get; set; } = "StockExchange";
    public static string StockFeederQueue { get; set; } = "Stock_Feeder_Queue";
    public static string StockValueInRoutingKey { get; set; } = "Stock_Value_In";
}
