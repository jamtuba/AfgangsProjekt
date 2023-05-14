using System.Collections.Generic;

namespace AP.GetStockPrices.Services;

public interface IRabbitMQPublisherService
{
    (string, byte[]) PublishRabbitMQ(List<CompanyInfo> companies);
}
