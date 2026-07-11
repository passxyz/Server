using System.ComponentModel.DataAnnotations;

namespace PassXYZ.Server.DTOs.Dashboard;

public class DashboardCreateRequest
{
    [Required]
    public string Id { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public object? Widgets { get; set; }

    public object? Tabs { get; set; }

    public object? Groups { get; set; }
}