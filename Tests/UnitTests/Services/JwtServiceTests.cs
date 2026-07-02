using Microsoft.Extensions.Configuration;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Tests.Services;

public class JwtServiceTests
{
    private readonly IJwtService _jwtService;

    public JwtServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-key-for-testing-only-keep-it-long",
                ["Jwt:ExpiresInMinutes"] = "60"
            })
            .Build();

        _jwtService = new JwtService(config);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        var token = _jwtService.GenerateToken("testuser");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnUsername()
    {
        var token = _jwtService.GenerateToken("testuser");
        var username = _jwtService.ValidateToken(token);

        Assert.Equal("testuser", username);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        var username = _jwtService.ValidateToken("invalid.token.here");

        Assert.Null(username);
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnNull()
    {
        var username = _jwtService.ValidateToken(string.Empty);

        Assert.Null(username);
    }

    [Fact]
    public void GetTokenExpiration_ShouldReturnValidDateTime()
    {
        var token = _jwtService.GenerateToken("testuser");
        var expiresAt = _jwtService.GetTokenExpiration(token);

        Assert.NotNull(expiresAt);
        Assert.True(expiresAt > DateTime.UtcNow);
        Assert.True(expiresAt < DateTime.UtcNow.AddMinutes(61));
    }

    [Fact]
    public void GetTokenExpiration_WithInvalidToken_ShouldReturnNull()
    {
        var expiresAt = _jwtService.GetTokenExpiration("invalid.token.here");

        Assert.Null(expiresAt);
    }
}