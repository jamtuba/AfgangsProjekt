using HtmlAgilityPack;
using System.Collections.Generic;

namespace AP.GetStockPrices.Services
{
    public interface IWebScraperService
    {
        List<CompanyInfo> GetNodes(HtmlNodeCollection nodes);
    }
}