namespace AP.ClassLibrary.Helpers;

public static class Endpoints
{
    public static string ExchangeName { get; set; } = "StockExchange";
    public static string StockFeederQueue { get; set; } = "Stock_Feeder_Queue";
    public static string StockValueInRoutingKey { get; set; } = "Stock_Value_In";
    public static string StockToDataBaseQueue { get; set; } = "Stock_to_Database_Queue";
    public static string TradesToDataBaseQueue { get; set; } = "Trades_to_Database_Queue";
    public static string CustomersHandlingDataBaseQueue { get; set; } = "Customers_Handling_Database_Queue";
    public static string AccountPrintsQueue { get; set; } = "Account_Prints_Queue";
    public static string InvoiceOutQueue { get; set; } = "Invoice_Out_Queue";
    public static string DataToWebsiteQueue { get; set; } = "Data_to_Website_Queue";
    public static string InvalidQueueName { get; set; } = "Invalid_queue";
    public static string InvalidRoutingKey { get; set; } = "Invalid";
}
