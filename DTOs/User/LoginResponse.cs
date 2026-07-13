namespace PassXYZ.Server.DTOs.User;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserProfileDto? User { get; set; }
}