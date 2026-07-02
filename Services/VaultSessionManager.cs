using System.Collections.Concurrent;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.Data;
using PassXYZLib;

namespace PassXYZ.Server.Services;

public enum DatabaseFileType
{
    PassXYZ,
    KeePass,
    Unknown
}

public class VaultSessionManager : IVaultSessionManager, IDisposable
{
    private readonly ConcurrentDictionary<string, PwDatabase> _sessions = new();
    private readonly ConcurrentDictionary<string, SessionInfo> _sessionInfo = new();
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly Timer _cleanupTimer;

    public VaultSessionManager(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _cleanupTimer = new Timer(CleanupExpiredSessions, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
    }

    public async Task<PwDatabase?> GetSession(string username)
    {
        if (_sessions.TryGetValue(username, out var db))
        {
            _sessionInfo.TryGetValue(username, out var info);
            info?.UpdateLastAccess();
            return db;
        }
        return null;
    }

    public async Task<bool> OpenSession(string username, string masterPassword)
    {
        try
        {
            var vaultPath = FindVaultPath(username);
            if (!File.Exists(vaultPath))
            {
                return false;
            }

            var ioInfo = new IOConnectionInfo { Path = vaultPath };
            var db = new PwDatabase();
            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword(masterPassword));

            if (IsDeviceLockEnabled(vaultPath))
            {
                var keyFilePath = GetKeyFilePath(username);
                if (File.Exists(keyFilePath))
                {
                    var keyFileIoInfo = new IOConnectionInfo { Path = keyFilePath };
                    compositeKey.AddUserKey(new KcpKeyFile(keyFileIoInfo));
                }
            }

            db.Open(ioInfo, compositeKey, null);
            _sessions.TryAdd(username, db);

            using (var scope = _serviceProvider.CreateScope())
            {
                var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                var user = usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    user.VaultFilePath = vaultPath;
                    user.IsDeviceLockEnabled = IsDeviceLockEnabled(vaultPath);
                    user.DatabaseFileType = vaultPath.EndsWith(PxDefs.xyz, StringComparison.OrdinalIgnoreCase)
                        ? "PassXYZ" : "KeePass";
                    usersDbContext.SaveChanges();
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task CloseSession(string username)
    {
        if (_sessions.TryRemove(username, out var db))
        {
            try
            {
                db.Close();
            }
            catch
            {
            }
        }
        _sessionInfo.TryRemove(username, out _);
    }

    public async Task<bool> IsSessionActive(string username)
    {
        return _sessions.ContainsKey(username);
    }

    public async Task<ExistingSession?> GetExistingSession(string username)
    {
        if (_sessionInfo.TryGetValue(username, out var info))
        {
            return new ExistingSession
            {
                DeviceInfo = info.DeviceInfo,
                LoginTime = info.LoginTime,
                IpAddress = info.IpAddress
            };
        }
        return null;
    }

    public async Task CreateSession(string username, string deviceInfo, string ipAddress)
    {
        _sessionInfo.AddOrUpdate(username,
            _ => new SessionInfo
            {
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress,
                LoginTime = DateTime.UtcNow,
                LastAccessTime = DateTime.UtcNow
            },
            (_, existing) =>
            {
                existing.DeviceInfo = deviceInfo;
                existing.IpAddress = ipAddress;
                existing.LoginTime = DateTime.UtcNow;
                existing.LastAccessTime = DateTime.UtcNow;
                return existing;
            });
    }

    public async Task InvalidateSession(string username)
    {
        await CloseSession(username);
    }

    private DatabaseFileType GetUserDatabaseType(string username)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            var user = usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null && !string.IsNullOrEmpty(user.DatabaseFileType))
            {
                if (user.DatabaseFileType.Equals("KeePass", StringComparison.OrdinalIgnoreCase))
                {
                    return DatabaseFileType.KeePass;
                }
                else if (user.DatabaseFileType.Equals("PassXYZ", StringComparison.OrdinalIgnoreCase))
                {
                    return DatabaseFileType.PassXYZ;
                }
            }

            if (user != null && !string.IsNullOrEmpty(user.VaultFilePath))
            {
                if (user.VaultFilePath.EndsWith(PxDefs.xyz, StringComparison.OrdinalIgnoreCase))
                {
                    return DatabaseFileType.PassXYZ;
                }
                else if (user.VaultFilePath.EndsWith(PxDefs.kdbx, StringComparison.OrdinalIgnoreCase))
                {
                    return DatabaseFileType.KeePass;
                }
            }
        }
        return DatabaseFileType.PassXYZ;
    }

    private string GetVaultPath(string username, DatabaseFileType fileType = DatabaseFileType.PassXYZ)
    {
        bool isDeviceLockEnabled = false;
        using (var scope = _serviceProvider.CreateScope())
        {
            var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            var user = usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
            isDeviceLockEnabled = user?.IsDeviceLockEnabled ?? false;
        }

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

    private string FindVaultPath(string username)
    {
        string? cachedVaultFilePath = null;
        using (var scope = _serviceProvider.CreateScope())
        {
            var usersDbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
            var user = usersDbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null && !string.IsNullOrEmpty(user.VaultFilePath) && File.Exists(user.VaultFilePath))
            {
                cachedVaultFilePath = user.VaultFilePath;
            }
        }

        if (cachedVaultFilePath != null)
        {
            return cachedVaultFilePath;
        }

        var fileType = GetUserDatabaseType(username);
        var primaryPath = GetVaultPath(username, fileType);
        if (File.Exists(primaryPath))
        {
            return primaryPath;
        }

        var fallbackType = fileType == DatabaseFileType.PassXYZ ? DatabaseFileType.KeePass : DatabaseFileType.PassXYZ;
        var fallbackPath = GetVaultPath(username, fallbackType);
        if (File.Exists(fallbackPath))
        {
            return fallbackPath;
        }

        var legacyPath = Path.Combine(
            _configuration["Data:VaultsPath"] ?? "./data/vaults",
            $"{username}.kdbx"
        );
        if (File.Exists(legacyPath))
        {
            return legacyPath;
        }

        return primaryPath;
    }

    private string GetKeyFilePath(string username)
    {
        var vaultsPath = _configuration["Data:VaultsPath"] ?? "./data/vaults";

        var head = PxFileTypeInfo.GetHead(PxFileType.PxKeyFile);
        var tail = PxFileTypeInfo.GetTail(PxFileType.PxKeyFile);
        var encodedUsername = Base58CheckEncoding.ToBase58String(username);

        return Path.Combine(vaultsPath, $"{head}{encodedUsername}{tail}");
    }

    private bool IsDeviceLockEnabled(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        return PxDefs.IsDeviceLockEnabled(fileName);
    }

    private void CleanupExpiredSessions(object? state)
    {
        var timeoutMinutes = int.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60");
        var timeout = TimeSpan.FromMinutes(timeoutMinutes);
        var now = DateTime.UtcNow;

        foreach (var (username, info) in _sessionInfo.ToArray())
        {
            if (now - info.LastAccessTime > timeout)
            {
                _ = CloseSession(username);
            }
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
        foreach (var (_, db) in _sessions.ToArray())
        {
            try
            {
                db.Close();
            }
            catch
            {
            }
        }
        _sessions.Clear();
        _sessionInfo.Clear();
    }

    private class SessionInfo
    {
        public string DeviceInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public DateTime LastAccessTime { get; set; }

        public void UpdateLastAccess()
        {
            LastAccessTime = DateTime.UtcNow;
        }
    }
}