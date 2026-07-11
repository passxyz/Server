using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.DTOs.Dashboard;

public class WidgetCreateRequest
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public object Position { get; set; } = null!;

    public object? Data { get; set; }
}