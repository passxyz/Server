using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.Data;

namespace PassXYZ.Server.Services;

public class UserService : IUserService
{
    private readonly IJwtService _jwtService;
    private readonly IVaultSessionManager _vaultSessionManager;
    private readonly UsersDbContext _usersDbContext;
    private readonly IConfiguration _configuration;

    public UserService(IJwtService jwtService, IVaultSessionManager vaultSessionManager, 
        UsersDbContext usersDbContext, IConfiguration configuration)
    {
        _jwtService = jwtService;
        _vaultSessionManager = vaultSessionManager;
        _usersDbContext = usersDbContext;
        _configuration = configuration;
    }

    public async Task<UserProfileDto?> GetUserProfile(string username)
    {
        var user = _usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return null;
        }

        return new UserProfileDto
        {
            Username = user.UserName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin ?? DateTime.MinValue,
            IsDeviceLockEnabled = false
        };
    }

    public async Task<UserProfileDto?> GetUserByEmail(string email)
    {
        var user = _usersDbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return null;
        }

        return new UserProfileDto
        {
            Username = user.UserName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLogin = user.LastLogin ?? DateTime.MinValue,
            IsDeviceLockEnabled = false
        };
    }

    public async Task<bool> CreateUser(SignUpRequest request, string email)
    {
        if (_usersDbContext.Users.Any(u => u.Email == email))
        {
            return false;
        }

        if (_usersDbContext.Users.Any(u => u.UserName == request.Username))
        {
            return false;
        }

        var user = new User
        {
            UserId = Guid.NewGuid().ToString(),
            Email = email,
            UserName = request.Username,
            CreatedAt = DateTime.UtcNow,
            LastLogin = null
        };

        _usersDbContext.Users.Add(user);
        await _usersDbContext.SaveChangesAsync();

        await CreateVault(request.Username, request.MasterPassword);

        return true;
    }

    public async Task<bool> DeleteUser(string username)
    {
        var user = _usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
        if (user == null)
        {
            return false;
        }

        await _vaultSessionManager.CloseSession(username);

        _usersDbContext.Users.Remove(user);
        await _usersDbContext.SaveChangesAsync();

        CleanupUserFiles(username);

        return true;
    }

    public async Task<List<string>> GetUsersList()
    {
        return _usersDbContext.Users.Select(u => u.UserName).ToList();
    }

    public async Task<LoginResult> Login(LoginRequest request, string email, string ipAddress)
    {
        var user = _usersDbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            return new LoginResult { Success = false };
        }

        var existingSession = await _vaultSessionManager.GetExistingSession(user.UserName);
        if (existingSession != null && !request.TakeOver.HasValue)
        {
            return new LoginResult { Success = false, ConflictSession = existingSession };
        }

        if (request.TakeOver.GetValueOrDefault())
        {
            await _vaultSessionManager.CloseSession(user.UserName);
        }

        var vaultOpened = await _vaultSessionManager.OpenSession(user.UserName, request.MasterPassword);
        if (!vaultOpened)
        {
            return new LoginResult { Success = false };
        }

        await _vaultSessionManager.CreateSession(user.UserName, request.DeviceInfo, ipAddress);

        var token = _jwtService.GenerateToken(user.UserName);
        var expiresAt = _jwtService.GetTokenExpiration(token);

        user.LastLogin = DateTime.UtcNow;
        await _usersDbContext.SaveChangesAsync();

        return new LoginResult
        {
            Success = true,
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.UserName
        };
    }

    public async Task Logout(string username)
    {
        await _vaultSessionManager.CloseSession(username);
    }

    private async Task CreateVault(string username, string masterPassword)
    {
        try
        {
            var vaultPath = GetVaultPath(username);
            var vaultsPath = Path.GetDirectoryName(vaultPath);
            if (!Directory.Exists(vaultsPath))
            {
                Directory.CreateDirectory(vaultsPath!);
            }

            var db = new PwDatabase();
            var ioInfo = new IOConnectionInfo { Path = vaultPath };

            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword(masterPassword));

            db.New(ioInfo, compositeKey);
            db.Save(null);
            db.Close();
        }
        catch
        {
        }
    }

    private void CleanupUserFiles(string username)
    {
        try
        {
            var vaultPath = GetVaultPath(username);
            if (File.Exists(vaultPath))
            {
                File.Delete(vaultPath);
            }

            var userDbPath = GetUserDbPath(username);
            if (File.Exists(userDbPath))
            {
                File.Delete(userDbPath);
            }
        }
        catch
        {
        }
    }

    private string GetVaultPath(string username)
    {
        var vaultsPath = _configuration["Data:VaultsPath"] ?? "/data/vaults";
        return Path.Combine(vaultsPath, $"{username}.kdbx");
    }

    private string GetUserDbPath(string username)
    {
        var userDatabasesPath = _configuration["Data:UserDatabasesPath"] ?? "/data/users";
        return Path.Combine(userDatabasesPath, $"{username}.db");
    }
}