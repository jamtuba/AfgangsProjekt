﻿using GetStockPrices.Models;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace GetStockPrices.Services;

public class WebScraperService : IWebScraperService
{
    public List<CompanyInfo> GetNodes(HtmlNodeCollection nodes)
    {
        //var pathToCompanyName = "//*[@id=\"137_company_name_0\"]";
        //var companyNameXPath = "//*[@id=\"137_company_name_0\"]/a";
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
                    Value = node.ChildNodes[1].InnerText
                });

                nodeCount++;
            }

        }
        return companies;
    }
}
