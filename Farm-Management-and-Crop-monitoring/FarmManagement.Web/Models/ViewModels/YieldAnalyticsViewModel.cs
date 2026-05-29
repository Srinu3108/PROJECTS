using FarmManagement.Web.Models.Entities;

namespace FarmManagement.Web.Models.ViewModels;

public class YieldAnalyticsViewModel
{
    public IEnumerable<string> CropNames { get; set; } = new List<string>();
    public IEnumerable<decimal> YieldValues { get; set; } = new List<decimal>();
    public IEnumerable<Harvest> Records { get; set; } = new List<Harvest>();
    public decimal AverageYield { get; set; }
    public decimal TotalYield { get; set; }
}