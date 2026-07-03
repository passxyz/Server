using System.Text;
using System.Text.Json;
using PassXYZ.Server.DTOs.User;
using PassXYZ.Server.DTOs.Vault;

namespace PassXYZ.Server.IntegrationTests.Controllers;

public class VaultControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public VaultControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetItems_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/groups/root/items");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SearchEntries_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/search?keyword=test");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEntry_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/entries/test-entry-id");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetGroup_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/groups/test-group-id");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEntry_WithoutToken_ShouldReturnUnauthorized()
    {
        var request = new NewEntryRequest
        {
            Name = "Test Entry"
        };

        var response = await _client.PostAsync("/api/vault/groups/root/entries",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_WithoutToken_ShouldReturnUnauthorized()
    {
        var request = new NewGroupRequest
        {
            Name = "Test Group"
        };

        var response = await _client.PostAsync("/api/vault/groups/root/groups",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEntry_WithoutToken_ShouldReturnUnauthorized()
    {
        var request = new EntryDto
        {
            Id = "test-id",
            Name = "Updated Entry"
        };

        var response = await _client.PutAsync("/api/vault/entries/test-id",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEntry_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/vault/entries/test-id");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetIcons_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/icons");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAttachments_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/entries/test-entry-id/attachments");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DownloadAttachment_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/vault/entries/test-entry-id/attachments/test-attachment");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadAttachment_WithoutToken_ShouldReturnUnauthorized()
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
        fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/png");
        content.Add(fileContent, "file", "test.png");

        var response = await _client.PostAsync("/api/vault/entries/test-entry-id/attachments", content);

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAttachment_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/vault/entries/test-entry-id/attachments/test-attachment");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}