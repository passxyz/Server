namespace PassXYZ.Server.DTOs.Portfolio;

public class StockUpdateRequest
{
    public string? Name { get; set; }

    public double? AvgCost { get; set; }

    public int? Quantity { get; set; }

    public double? TotalValue { get; set; }
}