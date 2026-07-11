using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.DTOs.Portfolio;

public class TransactionCreateRequest
{
    [Required]
    public string Symbol { get; set; } = null!;

    public string? Name { get; set; }

    public string? Date { get; set; }

    [Required]
    public double Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public string TransactionType { get; set; } = null!;

    public double? TotalValue { get; set; }

    public double? BaseValue { get; set; }

    public double? TransactionFee { get; set; }
}