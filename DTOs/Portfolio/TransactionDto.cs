namespace PassXYZ.Server.DTOs.Portfolio;

public class TransactionDto
{
    public int Id { get; set; }
    public string Symbol { get; set; } = null!;
    public string? Name { get; set; }
    public string Date { get; set; } = null!;
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string TransactionType { get; set; } = null!;
    public double TotalValue { get; set; }
    public double BaseValue { get; set; }
    public double TransactionFee { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string UpdatedAt { get; set; } = null!;
}