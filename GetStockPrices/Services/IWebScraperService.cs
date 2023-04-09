using GetStockPrices.Models;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace GetStockPrices.Services
{
    public interface IWebScraperService
    {
        List<CompanyInfo> GetNodes(HtmlNodeCollection nodes);
    }
}