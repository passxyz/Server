namespace PassXYZ.Server.DTOs.User;

public class SignUpRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MasterPassword { get; set; } = string.Empty;
    public bool IsDeviceLockEnabled { get; set; } = false;
}