using Microsoft.AspNetCore.SignalR;


namespace AP.API.Hubs;

public class SignalRHub : Hub
{
    public const string HubUrl = "/thehub";
    public async Task SendMessage(List<CompanyInfo> companies)
    {
        await Clients.All.SendAsync("NewStockData", companies);
    }
}
