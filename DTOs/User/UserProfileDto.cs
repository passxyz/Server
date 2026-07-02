namespace PassXYZ.Server.DTOs.User;

public class UserProfileDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public bool IsDeviceLockEnabled { get; set; }
}