using Microsoft.Extensions.Configuration;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Tests.Services;

public class VaultSessionManagerTests : IDisposable
{
    private readonly IVaultSessionManager _sessionManager;
    private readonly string _testVaultPath;
    private readonly string _testUsername = "testuser";

    public VaultSessionManagerTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Data:VaultsPath"] = Path.Combine(Path.GetTempPath(), "passxyz_test_vaults")
            })
            .Build();

        _sessionManager = new VaultSessionManager(config);
        _testVaultPath = Path.Combine(config["Data:VaultsPath"] ?? "/tmp", $"{_testUsername}.kdbx");
    }

    public void Dispose()
    {
        _sessionManager.CloseSession(_testUsername).Wait();
        
        if (Directory.Exists(Path.GetDirectoryName(_testVaultPath)))
        {
            try
            {
                Directory.Delete(Path.GetDirectoryName(_testVaultPath)!, true);
            }
            catch { }
        }
    }

    [Fact]
    public async Task OpenSession_WithValidPassword_ShouldReturnTrue()
    {
        CreateTestVault(_testUsername, "testpassword");

        var result = await _sessionManager.OpenSession(_testUsername, "testpassword");

        Assert.True(result);
    }

    [Fact]
    public async Task OpenSession_WithInvalidPassword_ShouldReturnFalse()
    {
        CreateTestVault(_testUsername, "testpassword");

        var result = await _sessionManager.OpenSession(_testUsername, "wrongpassword");

        Assert.False(result);
    }

    [Fact]
    public async Task OpenSession_WithNonExistentVault_ShouldReturnFalse()
    {
        var result = await _sessionManager.OpenSession("nonexistent", "password");

        Assert.False(result);
    }

    [Fact]
    public async Task GetSession_WithOpenSession_ShouldReturnDb()
    {
        CreateTestVault(_testUsername, "testpassword");
        await _sessionManager.OpenSession(_testUsername, "testpassword");

        var db = await _sessionManager.GetSession(_testUsername);

        Assert.NotNull(db);
    }

    [Fact]
    public async Task GetSession_WithClosedSession_ShouldReturnNull()
    {
        var db = await _sessionManager.GetSession(_testUsername);

        Assert.Null(db);
    }

    [Fact]
    public async Task CloseSession_ShouldRemoveSession()
    {
        CreateTestVault(_testUsername, "testpassword");
        await _sessionManager.OpenSession(_testUsername, "testpassword");

        await _sessionManager.CloseSession(_testUsername);

        var db = await _sessionManager.GetSession(_testUsername);
        Assert.Null(db);
    }

    [Fact]
    public async Task CreateSession_ShouldStoreSessionMetadata()
    {
        CreateTestVault(_testUsername, "testpassword");
        await _sessionManager.OpenSession(_testUsername, "testpassword");

        await _sessionManager.CreateSession(_testUsername, "test-device", "127.0.0.1");

        var session = await _sessionManager.GetExistingSession(_testUsername);
        Assert.NotNull(session);
        Assert.Equal("test-device", session.DeviceInfo);
        Assert.Equal("127.0.0.1", session.IpAddress);
    }

    [Fact]
    public async Task GetExistingSession_WithNoSession_ShouldReturnNull()
    {
        var session = await _sessionManager.GetExistingSession(_testUsername);

        Assert.Null(session);
    }

    private void CreateTestVault(string username, string password)
    {
        var vaultsPath = Path.GetDirectoryName(_testVaultPath);
        if (!Directory.Exists(vaultsPath))
        {
            Directory.CreateDirectory(vaultsPath!);
        }

        var db = new KeePassLib.PwDatabase();
        var ioInfo = new KeePassLib.Serialization.IOConnectionInfo { Path = _testVaultPath };
        var compositeKey = new KeePassLib.Keys.CompositeKey();
        compositeKey.AddUserKey(new KeePassLib.Keys.KcpPassword(password));

        db.New(ioInfo, compositeKey);
        db.Save(null);
        db.Close();
    }
}