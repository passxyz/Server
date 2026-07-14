using PassXYZ.Server.Data;
using PassXYZLib;
using System.Collections.Concurrent;

namespace PassXYZ.Server.Services;

public class DashboardDbContextFactory : IDashboardDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly ConcurrentDictionary<string, byte> _initializedDatabases = new();

    public DashboardDbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DashboardDbContext Create(string userId)
    {
        var dbPath = GetDashboardDbPath(userId);
        var db = new DashboardDbContext(dbPath);

        InitializeDatabaseIfNeeded(dbPath);

        return db;
    }

    private string GetDashboardDbPath(string userId)
    {
        var userDatabasesPath = _configuration["Data:UserDatabasesPath"] ?? "/data/users";
        var encodedUserId = Base58CheckEncoding.ToBase58String(userId);
        return Path.Combine(userDatabasesPath, $"{encodedUserId}.db");
    }

    private void InitializeDatabaseIfNeeded(string dbPath)
    {
        if (!_initializedDatabases.ContainsKey(dbPath))
        {
            if (_initializedDatabases.TryAdd(dbPath, 0))
            {
                using var db = new DashboardDbContext(dbPath);
                db.Database.EnsureCreated();
            }
        }
    }
}
