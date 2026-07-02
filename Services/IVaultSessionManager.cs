using KeePassLib;
using PassXYZ.Server.DTOs.User;

namespace PassXYZ.Server.Services;

public interface IVaultSessionManager
{
    Task<PwDatabase?> GetSession(string username);
    Task<bool> OpenSession(string username, string masterPassword);
    Task CloseSession(string username);
    Task<bool> IsSessionActive(string username);
    Task<ExistingSession?> GetExistingSession(string username);
    Task CreateSession(string username, string deviceInfo, string ipAddress);
    Task InvalidateSession(string username);
}