using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PassXYZ.Server.Data;

namespace PassXYZ.Server.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "passxyz_integration_tests_" + Guid.NewGuid());
            
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Secret"] = "test-secret-key-for-integration-testing-only",
                ["Jwt:ExpiresInMinutes"] = "60",
                ["Cloudflare:AccessEnabled"] = "false",
                ["Data:BasePath"] = tempPath,
                ["Data:UsersDbPath"] = Path.Combine(tempPath, "users.db"),
                ["Data:VaultsPath"] = Path.Combine(tempPath, "vaults"),
                ["Data:UserDatabasesPath"] = Path.Combine(tempPath, "users")
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<UsersDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<UsersDbContext>(options =>
            {
                var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                var dbPath = config["Data:UsersDbPath"] ?? "/data/users.db";
                options.UseSqlite($"Data Source={dbPath}");
            });

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<UsersDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }
}