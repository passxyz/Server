using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.Models;

public class Dashboard
{
    [Key]
    public string Id { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Widgets { get; set; }

    public string? Tabs { get; set; }

    public string? Groups { get; set; }

    [Required]
    public string CreatedAt { get; set; } = null!;

    [Required]
    public string UpdatedAt { get; set; } = null!;
}