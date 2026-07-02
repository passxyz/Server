using System.Collections.Concurrent;
using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using PassXYZ.Server.DTOs.User;

namespace PassXYZ.Server.Services;

public class VaultSessionManager : IVaultSessionManager, IDisposable
{
    private readonly ConcurrentDictionary<string, PwDatabase> _sessions = new();
    private readonly ConcurrentDictionary<string, SessionInfo> _sessionInfo = new();
    private readonly IConfiguration _configuration;
    private readonly Timer _cleanupTimer;

    public VaultSessionManager(IConfiguration configuration)
    {
        _configuration = configuration;
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
            var vaultPath = GetVaultPath(username);
            if (!File.Exists(vaultPath))
            {
                return false;
            }

            var ioInfo = new IOConnectionInfo { Path = vaultPath };
            var db = new PwDatabase();

            var compositeKey = new CompositeKey();
            compositeKey.AddUserKey(new KcpPassword(masterPassword));

            db.Open(ioInfo, compositeKey, null);
            _sessions.TryAdd(username, db);

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

    private string GetVaultPath(string username)
    {
        var vaultsPath = _configuration["Data:VaultsPath"] ?? "/data/vaults";
        return Path.Combine(vaultsPath, $"{username}.kdbx");
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