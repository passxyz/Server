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