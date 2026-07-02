using PassXYZ.Server.Services;
using PassXYZ.Server.Middleware;
using PassXYZ.Server.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PassXYZ.Server", Version = "v1" });
});

builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<IVaultSessionManager, VaultSessionManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVaultService, VaultService>();
builder.Services.AddScoped<UsersDbContext>();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

// 初始化数据库，确保表结构已创建
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    dbContext.Database.EnsureCreated();
}

// 打印数据文件解析后的绝对路径
var config = app.Services.GetRequiredService<IConfiguration>();
var paths = new (string Key, string Path)[]
{
    ("Data:UsersDbPath", config["Data:UsersDbPath"] ?? "./.data/users.db"),
    ("Data:VaultsPath", config["Data:VaultsPath"] ?? "./.data/vaults"),
    ("Data:UserDatabasesPath", config["Data:UserDatabasesPath"] ?? "./.data/users"),
};
foreach (var (key, path) in paths)
{
    var absolute = Path.GetFullPath(path);
    logger.LogInformation("{Key} = {AbsolutePath}", key, absolute);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PassXYZ.Server v1");
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<CloudflareAccessMiddleware>();
app.UseMiddleware<JwtAuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();