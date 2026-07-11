namespace PassXYZ.Server.DTOs.Portfolio;

public class TransactionUpdateRequest
{
    public string? Date { get; set; }

    public string? Symbol { get; set; }

    public string? Name { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public string? TransactionType { get; set; }

    public double? TotalValue { get; set; }

    public double? BaseValue { get; set; }

    public double? TransactionFee { get; set; }
}