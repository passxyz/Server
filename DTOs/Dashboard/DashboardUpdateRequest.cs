namespace PassXYZ.Server.DTOs.Dashboard;

public class DashboardUpdateRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public object? Widgets { get; set; }

    public object? Tabs { get; set; }

    public object? Groups { get; set; }
}