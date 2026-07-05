# ─── Stage 1: Build ────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 复制项目文件并恢复依赖（利用 Docker 层缓存）
COPY PassXYZ.Server.csproj .
RUN dotnet restore

# 复制源码并构建发布
COPY . .
RUN dotnet publish PassXYZ.Server.csproj -c Release -o /app/publish --no-restore

# ─── Stage 2: Runtime ──────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# 从 build 阶段复制已编译的程序
COPY --from=build /app/publish .

# 创建数据目录（会被 docker-compose volume 覆盖）
RUN mkdir -p .data

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PassXYZ.Server.dll"]
