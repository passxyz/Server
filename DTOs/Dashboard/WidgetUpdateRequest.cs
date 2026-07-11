namespace PassXYZ.Server.DTOs.Dashboard;

public class WidgetUpdateRequest
{
    public string? Title { get; set; }

    public object? Position { get; set; }

    public object? Data { get; set; }
}