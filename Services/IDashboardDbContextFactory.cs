using PassXYZ.Server.Data;

namespace PassXYZ.Server.Services;

public interface IDashboardDbContextFactory
{
    DashboardDbContext Create(string userId);
}