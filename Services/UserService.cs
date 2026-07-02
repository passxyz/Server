using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.Data;
using PassXYZLib;

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
            IsDeviceLockEnabled = user.IsDeviceLockEnabled
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
            IsDeviceLockEnabled = user.IsDeviceLockEnabled
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

        var user = new Data.User
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

    private async Task CreateVault(string username, string masterPassword, DatabaseFileType fileType = DatabaseFileType.PassXYZ)
    {
        try
        {
            var vaultPath = GetVaultPath(username, fileType);
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

            var user = _usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null)
            {
                user.VaultFilePath = vaultPath;
                user.DatabaseFileType = fileType == DatabaseFileType.PassXYZ ? "PassXYZ" : "KeePass";
                _usersDbContext.SaveChanges();
            }
        }
        catch
        {
        }
    }

    private void CleanupUserFiles(string username)
    {
        try
        {
            var passxyzPath = GetVaultPath(username, DatabaseFileType.PassXYZ);
            if (File.Exists(passxyzPath))
            {
                File.Delete(passxyzPath);
            }

            var keepassPath = GetVaultPath(username, DatabaseFileType.KeePass);
            if (File.Exists(keepassPath))
            {
                File.Delete(keepassPath);
            }

            var keyFilePath = GetKeyFilePath(username);
            if (File.Exists(keyFilePath))
            {
                File.Delete(keyFilePath);
            }

            var legacyPath = Path.Combine(
                _configuration["Data:VaultsPath"] ?? "./data/vaults",
                $"{username}.kdbx"
            );
            if (File.Exists(legacyPath))
            {
                File.Delete(legacyPath);
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

    private string GetVaultPath(string username, DatabaseFileType fileType = DatabaseFileType.PassXYZ)
    {
        var user = _usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
        bool isDeviceLockEnabled = user?.IsDeviceLockEnabled ?? false;

        var vaultsPath = _configuration["Data:VaultsPath"] ?? "./data/vaults";
        var encodedUsername = Base58CheckEncoding.ToBase58String(username);

        PxFileType pxFileType;
        if (fileType == DatabaseFileType.KeePass)
        {
            pxFileType = PxFileType.PwData;
        }
        else
        {
            pxFileType = isDeviceLockEnabled ? PxFileType.PxDataEx : PxFileType.PxData;
        }

        var head = PxFileTypeInfo.GetHead(pxFileType);
        var tail = PxFileTypeInfo.GetTail(pxFileType);

        return Path.Combine(vaultsPath, $"{head}{encodedUsername}{tail}");
    }

    private string GetKeyFilePath(string username)
    {
        var vaultsPath = _configuration["Data:VaultsPath"] ?? "./data/vaults";

        var head = PxFileTypeInfo.GetHead(PxFileType.PxKeyFile);
        var tail = PxFileTypeInfo.GetTail(PxFileType.PxKeyFile);
        var encodedUsername = Base58CheckEncoding.ToBase58String(username);

        return Path.Combine(vaultsPath, $"{head}{encodedUsername}{tail}");
    }

    private string GetUserDbPath(string username)
    {
        var userDatabasesPath = _configuration["Data:UserDatabasesPath"] ?? "/data/users";
        return Path.Combine(userDatabasesPath, $"{username}.db");
    }
}