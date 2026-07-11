using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.DTOs.Portfolio;

public class StockCreateRequest
{
    [Required]
    public string Symbol { get; set; } = null!;

    public string? Name { get; set; }

    public double AvgCost { get; set; }

    public int Quantity { get; set; }

    public double TotalValue { get; set; }

    public double CurrentPrice { get; set; }

    public double FiftyTwoWeekLow { get; set; }

    public double FiftyTwoWeekHigh { get; set; }

    public double DividendYield { get; set; }

    public double LatestDividend { get; set; }

    public string Strategy { get; set; } = "持有";

    public string? TradingView { get; set; }
}