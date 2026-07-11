using PassXYZ.Server.Data;

namespace PassXYZ.Server.Services;

public class DashboardDbContextFactory : IDashboardDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly HashSet<string> _initializedDatabases = new();

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
        return Path.Combine(userDatabasesPath, userId, "dashboards.db");
    }

    private void InitializeDatabaseIfNeeded(string dbPath)
    {
        if (!_initializedDatabases.Contains(dbPath))
        {
            lock (_initializedDatabases)
            {
                if (!_initializedDatabases.Contains(dbPath))
                {
                    using var db = new DashboardDbContext(dbPath);
                    db.Database.EnsureCreated();
                    _initializedDatabases.Add(dbPath);
                }
            }
        }
    }
}