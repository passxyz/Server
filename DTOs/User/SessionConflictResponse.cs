namespace PassXYZ.Server.DTOs.User;

public class SessionConflictResponse
{
    public string Error { get; set; } = "CONCURRENT_SESSION";
    public string Message { get; set; } = string.Empty;
    public ExistingSession ExistingSession { get; set; } = new();
    public string ConfirmUrl { get; set; } = string.Empty;
}

public class ExistingSession
{
    public string DeviceInfo { get; set; } = string.Empty;
    public DateTime LoginTime { get; set; }
    public string IpAddress { get; set; } = string.Empty;
}