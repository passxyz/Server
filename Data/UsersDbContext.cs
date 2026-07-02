using Microsoft.EntityFrameworkCore;

namespace PassXYZ.Server.Data;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<VaultMetadata> VaultMetadata { get; set; }

    private readonly IConfiguration _configuration;

    public UsersDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = _configuration["Data:UsersDbPath"] ?? "/data/users.db";
        EnsureDirectoryExists(dbPath);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<VaultMetadata>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<VaultMetadata>(vm => vm.UserId);
    }

    private void EnsureDirectoryExists(string dbPath)
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}

public class User
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsDeviceLockEnabled { get; set; } = false;
    public string VaultFilePath { get; set; } = string.Empty;
    public string DatabaseFileType { get; set; } = "PassXYZ";
}

public class VaultMetadata
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public int EntryCount { get; set; } = 0;
}