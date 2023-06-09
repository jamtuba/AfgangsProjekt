﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AP.GetStockPrices.Services;

public class WebScraperService : IWebScraperService
{
    public List<CompanyInfo> GetNodes(HtmlNodeCollection nodes)
    {
        var nodeId = "137";
        int nodeCount = 0;

        List<CompanyInfo> companies = new();

        var allNodes = nodes.SelectMany(n => n.ChildNodes).ToList();

        foreach (var node in allNodes)
        {
            var companyId = nodeId + "_company_name_" + nodeCount.ToString();

            var companyToAdd = new CompanyInfo();

            if (node.FirstChild.Id == companyId)
            {
                companies.Add(
                new CompanyInfo()
                {
                    CompanyId = nodeCount + 1,
                    CompanyName = node.FirstChild.InnerText,
                    Value = node.ChildNodes[1].InnerText,
                    Time = DateTime.Now.ToString("H:mm:ss d. MMMM yyyy",
                    CultureInfo.CreateSpecificCulture("da-DK"))
                }); 

                nodeCount++;
            }
        }
        return companies;
    }
}
