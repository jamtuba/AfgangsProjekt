using Microsoft.AspNetCore.SignalR;


namespace AP.API.Hubs;

public class SignalRHub : Hub
{
    public async Task SendMessage(List<CompanyInfo> companies)
    {
        await Clients.All.SendAsync("NewStockData", companies);
    }
}
