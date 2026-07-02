namespace PassXYZ.Server.DTOs.User;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string MasterPassword { get; set; } = string.Empty;
    public bool? TakeOver { get; set; } = false;
    public string DeviceInfo { get; set; } = string.Empty;
}