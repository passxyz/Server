using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.Models;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string Symbol { get; set; } = null!;

    public string? Name { get; set; }

    [Required]
    public string Date { get; set; } = null!;

    [Required]
    public double Price { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public string TransactionType { get; set; } = null!;

    [Required]
    public double TotalValue { get; set; }

    [Required]
    public double BaseValue { get; set; }

    public double TransactionFee { get; set; }

    [Required]
    public string CreatedAt { get; set; } = null!;

    [Required]
    public string UpdatedAt { get; set; } = null!;
}