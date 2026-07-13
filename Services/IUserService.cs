using PassXYZ.Server.DTOs.User;

namespace PassXYZ.Server.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetUserProfile(string username);
    Task<UserProfileDto?> GetUserByEmail(string email);
    Task<bool> CreateUser(SignUpRequest request, string email);
    Task<bool> DeleteUser(string username);
    Task<List<string>> GetUsersList();
    Task<LoginResult> Login(LoginRequest request, string? email, string ipAddress);
    Task Logout(string username);
    Task<bool> UpdateUserProfile(string username, UpdateProfileRequest request);
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Username { get; set; }
    public ExistingSession? ConflictSession { get; set; }
}