using Microsoft.EntityFrameworkCore;
using PassXYZ.Server.Models;

namespace PassXYZ.Server.Data;

public class DashboardDbContext : DbContext
{
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<Widget> Widgets { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    private readonly string _dbPath;

    public DashboardDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        EnsureDirectoryExists(_dbPath);
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dashboard>()
            .HasIndex(d => d.UserId);

        modelBuilder.Entity<Widget>()
            .HasIndex(w => w.DashboardId);

        modelBuilder.Entity<Stock>()
            .HasIndex(s => s.UserId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.UserId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Symbol);
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