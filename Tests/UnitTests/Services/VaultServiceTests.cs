using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Security;
using KeePassLib.Serialization;
using Microsoft.Extensions.Configuration;
using Moq;
using PassXYZ.Server.DTOs.Vault;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Tests.Services;

public class VaultServiceTests
{
    private readonly Mock<IVaultSessionManager> _mockSessionManager;
    private readonly VaultService _vaultService;

    public VaultServiceTests()
    {
        _mockSessionManager = new Mock<IVaultSessionManager>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Data:VaultsPath"] = Path.Combine(Path.GetTempPath(), "passxyz_test_vaults")
            })
            .Build();

        _vaultService = new VaultService(_mockSessionManager.Object, config);
    }

    [Fact]
    public async Task GetIcons_ShouldReturnAllIcons()
    {
        var icons = await _vaultService.GetIcons();

        Assert.NotNull(icons);
        Assert.NotEmpty(icons);
        Assert.True(icons.All(i => !string.IsNullOrEmpty(i.Name)));
        Assert.True(icons.All(i => i.Id >= 0));
    }

    [Fact]
    public async Task GetIcons_ShouldReturnAllPwIconValues()
    {
        var icons = await _vaultService.GetIcons();

        var expectedCount = Enum.GetValues(typeof(PwIcon)).Length;
        Assert.Equal(expectedCount, icons.Count);
    }

    [Fact]
    public async Task GetAttachments_WithNullSession_ShouldReturnEmptyList()
    {
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync((KeePassLib.PwDatabase?)null);

        var attachments = await _vaultService.GetAttachments("testuser", "test-entry-id");

        Assert.NotNull(attachments);
        Assert.Empty(attachments);
    }

    [Fact]
    public async Task GetAttachments_WithNonExistingEntry_ShouldReturnEmptyList()
    {
        var testPath = Path.Combine(Path.GetTempPath(), "passxyz_test", Guid.NewGuid().ToString() + ".kdbx");
        var db = new KeePassLib.PwDatabase();
        var ioInfo = new IOConnectionInfo { Path = testPath };
        var compositeKey = new CompositeKey();
        compositeKey.AddUserKey(new KcpPassword("test"));
        db.New(ioInfo, compositeKey);
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync(db);

        var attachments = await _vaultService.GetAttachments("testuser", Guid.NewGuid().ToString());

        Assert.NotNull(attachments);
        Assert.Empty(attachments);
    }

    [Fact]
    public async Task DownloadAttachment_WithNullSession_ShouldReturnNull()
    {
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync((KeePassLib.PwDatabase?)null);

        var data = await _vaultService.DownloadAttachment("testuser", "test-entry-id", "test-attachment");

        Assert.Null(data);
    }

    [Fact]
    public async Task DownloadAttachment_WithNonExistingEntry_ShouldReturnNull()
    {
        var testPath = Path.Combine(Path.GetTempPath(), "passxyz_test", Guid.NewGuid().ToString() + ".kdbx");
        var db = new KeePassLib.PwDatabase();
        var ioInfo = new IOConnectionInfo { Path = testPath };
        var compositeKey = new CompositeKey();
        compositeKey.AddUserKey(new KcpPassword("test"));
        db.New(ioInfo, compositeKey);
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync(db);

        var data = await _vaultService.DownloadAttachment("testuser", Guid.NewGuid().ToString(), "test-attachment");

        Assert.Null(data);
    }

    [Fact]
    public async Task DeleteAttachment_WithNullSession_ShouldReturnFalse()
    {
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync((KeePassLib.PwDatabase?)null);

        var result = await _vaultService.DeleteAttachment("testuser", "test-entry-id", "test-attachment");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAttachment_WithNonExistingEntry_ShouldReturnFalse()
    {
        var testPath = Path.Combine(Path.GetTempPath(), "passxyz_test", Guid.NewGuid().ToString() + ".kdbx");
        var db = new KeePassLib.PwDatabase();
        var ioInfo = new IOConnectionInfo { Path = testPath };
        var compositeKey = new CompositeKey();
        compositeKey.AddUserKey(new KcpPassword("test"));
        db.New(ioInfo, compositeKey);
        _mockSessionManager.Setup(s => s.GetSession("testuser"))
            .ReturnsAsync(db);

        var result = await _vaultService.DeleteAttachment("testuser", Guid.NewGuid().ToString(), "test-attachment");

        Assert.False(result);
    }
}