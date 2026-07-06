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

    [Fact]
    public void GetIconData_DefaultEntryIcon_ShouldReturnKeyIcon()
    {
        var (data, contentType) = KeePassIcons.GetIconData(PwIcon.Key);
        
        Assert.NotNull(data);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        Assert.Contains("Key", KeePassIcons.GetIconName(PwIcon.Key));
        Console.WriteLine($"PwIcon.Key (value={((int)PwIcon.Key)}) returns SVG with content type: {contentType}");
        Console.WriteLine($"SVG content preview: {svgContent.Substring(0, Math.Min(100, svgContent.Length))}...");
    }

    [Fact]
    public void GetIconData_DefaultGroupIcon_ShouldReturnFolderIcon()
    {
        var (data, contentType) = KeePassIcons.GetIconData(PwIcon.Folder);
        
        Assert.NotNull(data);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        Assert.Contains("Folder", KeePassIcons.GetIconName(PwIcon.Folder));
        Console.WriteLine($"PwIcon.Folder (value={((int)PwIcon.Folder)}) returns SVG with content type: {contentType}");
        Console.WriteLine($"SVG content preview: {svgContent.Substring(0, Math.Min(100, svgContent.Length))}...");
    }

    [Fact]
    public void GetIconData_StarIcon_ShouldReturnStarSvg()
    {
        var (data, contentType) = KeePassIcons.GetIconData(PwIcon.Star);
        
        Assert.NotNull(data);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
        Console.WriteLine($"PwIcon.Star (value={((int)PwIcon.Star)}) returns SVG with content type: {contentType}");
        Console.WriteLine($"SVG content preview: {svgContent.Substring(0, Math.Min(100, svgContent.Length))}...");
    }

    [Fact]
    public void KeePassIconsMapping_VerifyEnumValues()
    {
        Console.WriteLine("=== PwIcon Enum Values ===");
        foreach (var icon in Enum.GetValues(typeof(PwIcon)).Cast<PwIcon>())
        {
            var (data, contentType) = KeePassIcons.GetIconData(icon);
            Console.WriteLine($"{(int)icon} = {icon,-25} -> ContentType: {contentType,-18} -> HasData: {!string.IsNullOrEmpty(data)}");
        }
    }

    [Fact]
    public void GetIconData_DtoConversion_EntryDefaultIcon()
    {
        var db = new KeePassLib.PwDatabase();
        
        var entry = new PwEntry(true, true);
        entry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "Test Entry"));
        
        Console.WriteLine($"Entry default IconId: {entry.IconId} (value = {(int)entry.IconId})");
        
        var (iconData, contentType) = KeePassIcons.GetIconData(entry.IconId);
        
        Assert.NotNull(iconData);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(iconData));
        Console.WriteLine($"Icon returned: {entry.IconId}");
        Console.WriteLine($"SVG content: {svgContent}");
    }

    [Fact]
    public void GetIconData_DtoConversion_GroupDefaultIcon()
    {
        var group = new PwGroup(true, true, "Test Group", PwIcon.Folder);
        
        Console.WriteLine($"Group default IconId: {group.IconId} (value = {(int)group.IconId})");
        
        var (iconData, contentType) = KeePassIcons.GetIconData(group.IconId);
        
        Assert.NotNull(iconData);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(iconData));
        Console.WriteLine($"Icon returned: {group.IconId}");
        Console.WriteLine($"SVG content: {svgContent}");
    }

    [Fact]
    public void GetIconData_DtoConversion_StarIcon()
    {
        var entry = new PwEntry(true, true);
        entry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "Test Entry"));
        entry.IconId = PwIcon.Star;
        
        Console.WriteLine($"Entry IconId set to: {entry.IconId} (value = {(int)entry.IconId})");
        
        var (iconData, contentType) = KeePassIcons.GetIconData(entry.IconId);
        
        Assert.NotNull(iconData);
        Assert.Equal("image/svg+xml", contentType);
        var svgContent = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(iconData));
        Console.WriteLine($"Icon returned: {entry.IconId}");
        Console.WriteLine($"SVG content: {svgContent}");
    }

    [Fact]
    public void GetIcons_ShouldReturnCorrectIconData()
    {
        var icons = _vaultService.GetIcons().Result;
        
        var folderIcon = icons.FirstOrDefault(i => i.Id == (int)PwIcon.Folder);
        Assert.NotNull(folderIcon);
        Assert.Equal("Folder", folderIcon.Name);
        Assert.Equal("image/svg+xml", folderIcon.ContentType);
        Console.WriteLine($"Folder icon (id={folderIcon.Id}): has data = {!string.IsNullOrEmpty(folderIcon.Data)}");

        var starIcon = icons.FirstOrDefault(i => i.Id == (int)PwIcon.Star);
        Assert.NotNull(starIcon);
        Assert.Equal("Star", starIcon.Name);
        Assert.Equal("image/svg+xml", starIcon.ContentType);
        Console.WriteLine($"Star icon (id={starIcon.Id}): has data = {!string.IsNullOrEmpty(starIcon.Data)}");

        var keyIcon = icons.FirstOrDefault(i => i.Id == (int)PwIcon.Key);
        Assert.NotNull(keyIcon);
        Assert.Equal("Key", keyIcon.Name);
        Assert.Equal("image/svg+xml", keyIcon.ContentType);
        Console.WriteLine($"Key icon (id={keyIcon.Id}): has data = {!string.IsNullOrEmpty(keyIcon.Data)}");
    }
}