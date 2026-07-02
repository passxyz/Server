namespace PassXYZ.Server.Services;

public interface IJwtService
{
    string GenerateToken(string username);
    string? ValidateToken(string token);
    DateTime? GetTokenExpiration(string token);
}