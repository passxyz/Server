using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.Models;

public class Widget
{
    [Key]
    public string Id { get; set; } = null!;

    [Required]
    public string DashboardId { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Position { get; set; } = null!;

    public string? Data { get; set; }

    [Required]
    public string CreatedAt { get; set; } = null!;

    [Required]
    public string UpdatedAt { get; set; } = null!;
}