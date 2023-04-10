﻿namespace AP.ClassLibrary.Model;

public class CompanyInfo
{
    public int CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? Value { get; set; }
    public string Time { get; set; } = DateTime.Now.ToString("G");
}
