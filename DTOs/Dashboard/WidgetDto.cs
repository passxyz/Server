namespace PassXYZ.Server.DTOs.Dashboard;

public class WidgetDto
{
    public string Id { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public object Position { get; set; } = null!;
    public object? Data { get; set; }
}