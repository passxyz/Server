using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PassXYZ.Server.Data;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Tests.Services;

public class UserServiceTests : IDisposable
{
    private readonly UsersDbContext _dbContext;
    private readonly UserService _userService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IVaultSessionManager> _mockVaultSessionManager;

    public UserServiceTests()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), "passxyz_test", Guid.NewGuid().ToString() + ".db");
        
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Data:VaultsPath"] = Path.Combine(Path.GetTempPath(), "passxyz_test_vaults"),
                ["Data:UserDatabasesPath"] = Path.Combine(Path.GetTempPath(), "passxyz_test_users"),
                ["Data:UsersDbPath"] = dbPath
            })
            .Build();

        _dbContext = new UsersDbContext(config);
        _dbContext.Database.EnsureCreated();

        _mockJwtService = new Mock<IJwtService>();
        _mockVaultSessionManager = new Mock<IVaultSessionManager>();

        _userService = new UserService(
            _mockJwtService.Object,
            _mockVaultSessionManager.Object,
            _dbContext,
            config
        );

        _dbContext.Users.Add(new Data.User
        {
            UserId = "1",
            Email = "test@example.com",
            UserName = "testuser",
            CreatedAt = DateTime.UtcNow,
            IsDeviceLockEnabled = false
        });
        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Fact]
    public async Task UpdateUserProfile_WithExistingUser_ShouldUpdateProfile()
    {
        var request = new UpdateProfileRequest
        {
            IsDeviceLockEnabled = true
        };

        var result = await _userService.UpdateUserProfile("testuser", request);

        Assert.True(result);

        var user = _dbContext.Users.First(u => u.UserName == "testuser");
        Assert.True(user.IsDeviceLockEnabled);
    }

    [Fact]
    public async Task UpdateUserProfile_WithNonExistingUser_ShouldReturnFalse()
    {
        var request = new UpdateProfileRequest
        {
            IsDeviceLockEnabled = true
        };

        var result = await _userService.UpdateUserProfile("nonexistent", request);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUserProfile_WithNullIsDeviceLockEnabled_ShouldNotChangeValue()
    {
        var request = new UpdateProfileRequest
        {
            IsDeviceLockEnabled = null
        };

        var result = await _userService.UpdateUserProfile("testuser", request);

        Assert.True(result);

        var user = _dbContext.Users.First(u => u.UserName == "testuser");
        Assert.False(user.IsDeviceLockEnabled);
    }
}