using System.Text;
using System.Text.Json;
using PassXYZ.Server.DTOs.User;

namespace PassXYZ.Server.IntegrationTests.Controllers;

public class UserControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory _factory;

    public UserControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SignUp_WithValidRequest_ShouldReturnOk()
    {
        var request = new SignUpRequest
        {
            Username = "testuser1",
            MasterPassword = "testpassword123"
        };

        var response = await _client.PostAsync("/api/user/signup", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_WithExistingUser_ShouldReturnConflict()
    {
        var request = new SignUpRequest
        {
            Username = "testuser2",
            MasterPassword = "testpassword123"
        };

        await _client.PostAsync("/api/user/signup", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        var response = await _client.PostAsync("/api/user/signup", 
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/user/profile");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsync("/api/user/logout", new StringContent("{}", Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUsersList_WithoutToken_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/user/list");

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_WithoutToken_ShouldReturnUnauthorized()
    {
        var request = new UpdateProfileRequest
        {
            IsDeviceLockEnabled = true
        };

        var response = await _client.PutAsync("/api/user/profile",
            new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));

        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}