namespace PassXYZ.Server.DTOs.Dashboard;

public class DashboardDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public object? Widgets { get; set; }
    public object? Tabs { get; set; }
    public object? Groups { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string UpdatedAt { get; set; } = null!;
}