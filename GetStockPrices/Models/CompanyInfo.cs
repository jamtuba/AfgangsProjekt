using System;

namespace GetStockPrices.Models;

public record CompanyInfo
{
    public int CompanyId { get; init; }
    public string CompanyName { get; init; }
    public string Value { get; init; }
    public DateTime Time { get; init; } = DateTime.Now;
}