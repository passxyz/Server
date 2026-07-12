# PassXYZ Vault 3 Server

PassXYZ 和 Finanalyzer 生态系统应用的共享后端 API 服务，提供密码库管理、用户认证和仪表板/投资组合服务。

## 概述

PassXYZ.Server 是一个基于 ASP.NET Core 构建的统一后端 API 服务，作为以下应用的中央数据层：

- **passxyz-web** - 兼容 KeePass 的基于 Web 的密码管理器
- **finanalyzer-app** - 具有仪表板功能的财务分析工作台

服务器处理认证、KeePass 数据库操作、会话管理，并为两个前端应用提供 RESTful API。

**主要功能：**

- 多层认证（Cloudflare Access + JWT + 主密码）
- 兼容 KeePass 的密码库操作
- 单点登录强制（防止并发登录）
- 设备锁支持
- 密码库管理的 RESTful API 端点
- 仪表板和投资组合管理 API
- 可配置的 CORS 策略
- Swagger API 文档

## 技术栈

| 组件 | 版本 | 描述 |
| ---------------------- | -------- | ------------------------------- |
| .NET | 10.0 | 跨平台运行时 |
| ASP.NET Core | 10.0 | Web API 框架 |
| Swashbuckle.AspNetCore | 10.2.3 | Swagger API 文档 |
| PassXYZLib | 3.0.1 | KeePass 功能扩展 |
| KPCLib | 2.0.4 | KeePassLib .NET Standard |
| SQLite | 内置 | 轻量级数据库 |

## 项目结构

```
PassXYZ.Server/
├── Controllers/
│   ├── ConfigController.cs    # 配置文件端点 ✅
│   ├── DashboardController.cs # 仪表板 API ✅
│   ├── PortfolioController.cs # 投资组合 API ✅
│   ├── UserController.cs      # 用户管理 API ✅
│   └── VaultController.cs     # 密码库管理 API ✅
├── Middleware/
│   ├── CloudflareAccessMiddleware.cs   # Cloudflare Access 认证 ✅
│   └── JwtAuthenticationMiddleware.cs  # JWT 认证 ✅
├── Services/
│   ├── IJwtService.cs                    ✅
│   ├── JwtService.cs                     ✅
│   ├── IVaultSessionManager.cs           ✅
│   ├── VaultSessionManager.cs            ✅
│   ├── IUserService.cs                   ✅
│   ├── UserService.cs                    ✅
│   ├── IVaultService.cs                  ✅
│   ├── VaultService.cs                   ✅
│   ├── IDashboardService.cs              ✅
│   ├── DashboardService.cs               ✅
│   ├── IPortfolioService.cs              ✅
│   └── PortfolioService.cs               ✅
├── Data/
│   ├── UsersDbContext.cs                 ✅
│   ├── UsersDbContextFactory.cs          ✅
│   └── DashboardDbContext.cs             ✅
├── Properties/
│   └── launchSettings.json    # 启动配置 ✅
├── PassXYZ.Server.csproj      # 项目文件 ✅
├── Program.cs                 # 应用入口点 ✅
├── appsettings.json           # 应用配置 ✅
└── appsettings.Development.json  # 开发环境配置 ✅
```

**图例：** ✅ = 已实现，`(planned)` = 尚未实现

## 快速开始

### 前提条件

- .NET 10 SDK 或更高版本
- Node.js 18+（用于前端应用）
- pnpm 包管理器（用于前端应用）

### 安装

```bash
git clone https://github.com/finanalyzer/server.git PassXYZ.Server
# 导航到服务器目录（从 monorepo 根目录）
cd PassXYZ.Server

# 还原依赖项
dotnet restore
```

<br />

### 开发

```bash
# 启动服务器
dotnet run
```

### 访问地址

| 服务 | URL |
| ------------ | ----------------------------------------------- |
| HTTP | `http://localhost:5182` |
| HTTPS | `https://localhost:7060` |
| Swagger UI | `http://localhost:5182/swagger` |
| OpenAPI JSON | `http://localhost:5182/swagger/v1/swagger.json` |

### 构建

```bash
# 生产环境构建
dotnet build --configuration Release

# 构建并运行
dotnet run --configuration Release
```

## 认证架构

### 多层安全机制

PassXYZ.Server 实现了三层认证系统：

1. **Cloudflare Access** - 身份验证（基于电子邮件）
2. **本地 JWT** - 会话认证
3. **主密码** - 数据解密（KeePass 数据库）

### 认证流程

```
1. 用户 → Cloudflare Access → Cloudflare JWT（包含电子邮件）
2. 前端 → POST /api/user/login（Cloudflare JWT + 凭据）
3. 后端 → 验证 Cloudflare JWT → 从 users.db 查询用户
4. 检查活动会话 → 如果存在则返回 409 Conflict
5. 使用主密码打开 KeePass 数据库 → 创建会话
6. 返回本地 JWT → 存储在 localStorage
7. 所有后续请求 → Authorization: Bearer <local-jwt>
```

### 数据流

```
前端页面 → REST API → VaultController → VaultService 
                                          → VaultSessionManager → PwDatabase
                                                                      ↓
                                                                UsersDbContext
```

### 会话管理

- Token 存储在前端 localStorage 中，键为 `passxyz-token`
- 会话超时：1 小时（可配置）
- 并发登录检测：返回 409 Conflict 及会话详情
- 自动锁定：前端在超时触发 `POST /api/user/logout`
- 设备锁：登录时需要 WebAuthn 验证

### 匿名端点

以下端点无需认证即可访问：

| 端点 | 描述 |
| ------------------- | --------------------- |
| `/api/user/login` | 用户登录 |
| `/api/user/signup` | 用户注册 |
| `/api/apps.json` | 应用配置 |
| `/api/agents.json` | 代理配置 |
| `/api/widgets.json` | 组件配置 |

## API 参考

### 用户管理 (`/api/user`)

| 方法 | 端点 | 描述 | 认证方式 |
| ------ | ---------------------- | ------------------- | -------------- |
| POST | `/api/user/signup` | 用户注册 | Cloudflare JWT |
| POST | `/api/user/login` | 用户登录 | Cloudflare JWT |
| POST | `/api/user/logout` | 用户登出 | 本地 JWT |
| GET | `/api/user/profile` | 获取用户资料 | 本地 JWT |
| PUT | `/api/user/profile` | 更新用户资料 | 本地 JWT |
| DELETE | `/api/user/{username}` | 删除用户 | 本地 JWT |
| GET | `/api/user/list` | 获取用户列表 | 本地 JWT |

#### 登录请求

```json
POST /api/user/login
Authorization: Bearer <cloudflare-jwt>

{
  "username": "alice",
  "masterPassword": "user-password",
  "takeOver": false,
  "deviceInfo": "Windows Chrome"
}
```

#### 登录响应（成功）

```json
{
  "token": "local-jwt-token",
  "expiresAt": "2026-07-02T12:00:00Z",
  "username": "alice",
  "email": "alice@example.com"
}
```

#### 登录响应（冲突）

```json
HTTP 409 Conflict

{
  "error": "CONCURRENT_SESSION",
  "message": "另一台设备已登录",
  "existingSession": {
    "deviceInfo": "Windows Chrome",
    "loginTime": "2026-07-02T10:00:00Z",
    "ipAddress": "192.168.1.100"
  },
  "confirmUrl": "/api/user/login?takeOver=true"
}
```

### 密码库管理 (`/api/vault`)

| 方法 | 端点 | 描述 | 认证方式 |
| ------ | ------------------------------------------ | ---------------------- | -------------- |
| GET | `/api/vault/groups/{groupId}/items` | 获取分组中的项目 | 本地 JWT |
| GET | `/api/vault/items/{itemId}` | 获取单个项目 | 本地 JWT |
| GET | `/api/vault/entries/{entryId}` | 获取条目详情 | 本地 JWT |
| GET | `/api/vault/groups/{groupId}` | 获取分组详情 | 本地 JWT |
| GET | `/api/vault/search?keyword={keyword}` | 搜索条目 | 本地 JWT |
| GET | `/api/vault/icons` | 获取图标列表 | 本地 JWT |
| POST | `/api/vault/groups/{groupId}/entries` | 创建条目 | 本地 JWT |
| POST | `/api/vault/groups/{parentGroupId}/groups` | 创建分组 | 本地 JWT |
| PUT | `/api/vault/entries/{entryId}` | 更新条目 | 本地 JWT |
| PUT | `/api/vault/groups/{groupId}` | 更新分组 | 本地 JWT |
| DELETE | `/api/vault/entries/{entryId}` | 删除条目 | 本地 JWT |
| DELETE | `/api/vault/groups/{groupId}` | 删除分组 | 本地 JWT |
| POST | `/api/vault/change-password` | 修改主密码 | 本地 JWT |

### 附件管理

| 方法 | 端点 | 描述 |
| ------ | --------------------------------------------------------- | ------------------- |
| GET | `/api/vault/entries/{entryId}/attachments` | 获取附件列表 |
| GET | `/api/vault/entries/{entryId}/attachments/{attachmentId}` | 下载附件 |
| POST | `/api/vault/entries/{entryId}/attachments` | 上传附件 |
| DELETE | `/api/vault/entries/{entryId}/attachments/{attachmentId}` | 删除附件 |

### 配置端点（公开）

| 端点 | 描述 |
| ----------------------- | ------------------------- |
| GET `/api/apps.json` | 获取应用配置 |
| GET `/api/agents.json` | 获取代理配置 |
| GET `/api/widgets.json` | 获取组件配置 |

### 仪表板 API

| 方法 | 端点 | 描述 |
| ------ | ------------------------------------------------------ | --------------------- |
| GET | `/api/v1/dashboard` | 获取所有仪表板 |
| POST | `/api/v1/dashboard` | 创建新仪表板 |
| GET | `/api/v1/dashboard/{dashboard_id}` | 获取单个仪表板 |
| PUT | `/api/v1/dashboard/{dashboard_id}` | 更新仪表板 |
| DELETE | `/api/v1/dashboard/{dashboard_id}` | 删除仪表板 |
| GET | `/api/v1/dashboard/{dashboard_id}/widgets` | 获取仪表板组件 |
| POST | `/api/v1/dashboard/{dashboard_id}/widgets` | 添加组件 |
| PUT | `/api/v1/dashboard/{dashboard_id}/widgets/{widget_id}` | 更新组件 |
| DELETE | `/api/v1/dashboard/{dashboard_id}/widgets/{widget_id}` | 删除组件 |

### 投资组合 API

| 方法 | 端点 | 描述 |
| ------ | ------------------------------------------------- | ------------------ |
| GET | `/api/v1/portfolio/stocks` | 获取所有股票 |
| POST | `/api/v1/portfolio/stocks` | 添加股票 |
| GET | `/api/v1/portfolio/stocks/{symbol}` | 获取单个股票 |
| PUT | `/api/v1/portfolio/stocks/{symbol}` | 更新股票 |
| DELETE | `/api/v1/portfolio/stocks/{symbol}` | 删除股票 |
| GET | `/api/v1/portfolio/transactions` | 获取交易记录 |
| POST | `/api/v1/portfolio/transactions` | 创建交易记录 |
| PUT | `/api/v1/portfolio/transactions/{transaction_id}` | 更新交易记录 |
| DELETE | `/api/v1/portfolio/transactions/{transaction_id}` | 删除交易记录 |

## CORS 配置

PassXYZ.Server 支持通过 `appsettings.json` 进行 CORS 配置：

### 默认配置

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:5174"
    ],
    "AllowedMethods": [
      "GET",
      "POST",
      "PUT",
      "DELETE",
      "OPTIONS"
    ],
    "AllowedHeaders": [
      "Authorization",
      "Content-Type",
      "Accept"
    ],
    "AllowCredentials": true,
    "ExposedHeaders": []
  }
}
```

### 环境变量

通过环境变量覆盖配置：

```bash
export Cors__AllowedOrigins__0="http://localhost:5173"
export Cors__AllowedOrigins__1="http://localhost:5174"
export Cors__AllowCredentials="true"
```

### 开发环境配置

`appsettings.Development.json` 包含额外的源：

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:5174",
      "http://localhost:5182"
    ]
  }
}
```

## 客户端集成

### passxyz-web

**配置：**

- 基础 URL: `/api`（通过 Vite 代理）
- Token 存储: `localStorage`，键为 `passxyz-token`
- 认证类型: `passxyz-jwt`

**Vite 代理 (`vite.config.ts`)：**

```typescript
proxy: {
  "/api": {
    target: "http://localhost:5182",
    changeOrigin: true,
    secure: false,
  },
  "/app": {
    target: "http://localhost:5174",
    changeOrigin: true,
    secure: false,
    rewrite: (path) => path.replace(/^\/app/, ""),
  },
},
```

### finanalyzer-app

**配置：**

- 基础 URL: `/api`（通过 Vite 代理）
- 认证类型: `passxyz-jwt`
- Token 来源: 从 localStorage 读取 `passxyz-token`

**连接配置：**

| 字段 | 值 |
| --------- | -------------- |
| Name | PassXYZ.Server |
| URL | `/api` |
| Auth Type | passxyz-jwt |

**认证流程：**

1. 用户通过 passxyz-web 登录
2. JWT token 存储在 `localStorage` 的 `passxyz-token` 键中
3. finanalyzer-app 从 `localStorage` 读取 token
4. token 自动添加到 Authorization 头部

```typescript
const token = localStorage.getItem('passxyz-token');
if (token) {
  headers['Authorization'] = `Bearer ${token}`;
}
```

**401 处理：**

当 API 返回 401 时，finanalyzer-app 清除 token 并重定向：

```typescript
export function handleUnauthorized(): void {
  localStorage.removeItem('passxyz-token');
  localStorage.removeItem('passxyz-user');
  window.location.href = '/vault/#/login';
}
```

### 本地开发设置

| 应用 | 端口 | 描述 |
| --------------- | ---- | ------------------------- |
| passxyz-web | 5173 | 密码管理器前端 |
| finanalyzer-app | 5174 | 仪表板应用 |
| PassXYZ.Server | 5182 | 后端 API 服务 |

#### 代理集成模式（推荐）

所有应用通过 passxyz-web 代理共享同一来源：

```
http://localhost:5173/vault/          → passxyz-web
http://localhost:5173/app/            → passxyz-web 代理 → finanalyzer-app
http://localhost:5173/api/            → passxyz-web 代理 → PassXYZ.Server
```

**步骤：**

1. 启动 PassXYZ.Server: `cd PassXYZ.Server && dotnet run`
2. 启动 finanalyzer-app: `cd finanalyzer-app && pnpm run dev`
3. 启动 passxyz-web: `cd passxyz-web && pnpm run dev`
4. 访问 `http://localhost:5173/vault/#/login` 登录
5. 访问 `http://localhost:5173/app/` 访问仪表板

#### 独立开发模式

分别运行每个应用并手动同步 token：

1. 按照代理集成模式的步骤 1-4 操作
2. 从 passxyz-web 的 localStorage 复制 `passxyz-token`
3. 通过浏览器控制台在 finanalyzer-app 的 localStorage 中设置 token

## Cloudflare Tunnel 设置

### 配置 (`~/.cloudflared/config.yml`)

```yaml
tunnel: <TUNNEL_ID>
credentials-file: /home/user/.cloudflared/<TUNNEL_ID>.json
ingress:
  - hostname: yourdomain.com
    path: ^/api/.*
    service: http://localhost:5182

  - hostname: yourdomain.com
    path: ^/app/.*
    service: http://localhost:5174

  - hostname: yourdomain.com
    path: ^/vault/.*
    service: http://localhost:5173

  - service: http_status:404
```

### 启动隧道

```bash
cloudflared tunnel --no-prechecks run
```

### 访问地址

| 服务 | 本地 URL | 隧道 URL |
| --------------- | ------------------------------ | -------------------------------- |
| passxyz-web | `http://localhost:5173/vault/` | `https://yourdomain.com/vault/` |
| finanalyzer-app | `http://localhost:5174/` | `https://yourdomain.com/app/` |
| PassXYZ.Server | `http://localhost:5182/api/` | `https://yourdomain.com/api/` |

## 安全

- **密码保护：** 传输过程中使用 HTTPS 加密，静态存储时使用 KeePass 原生加密
- **JWT 安全：** 使用强密钥签名，合理的过期时间（1 小时）
- **会话超时：** 可配置的空闲超时时间，自动锁定
- **单点登录：** 防止在多个设备上并发登录
- **输入验证：** ASP.NET Core 模型验证所有 API 输入
- **设备锁：** WebAuthn 标准用于生物识别验证
- **XSS 防护：** Markdown 渲染时进行 HTML 标签清理
- **OTP 安全：** TOTP 密钥仅在客户端处理

## 开发测试

### 禁用认证（仅开发环境）

在 `Program.cs` 中临时注释掉认证中间件：

```csharp
// app.UseMiddleware<CloudflareAccessMiddleware>();
// app.UseMiddleware<JwtAuthenticationMiddleware>();
```

**注意：** 仅用于开发调试。生产环境必须启用认证。

### Swagger 文档

访问 `http://localhost:5182/swagger` 获取交互式 API 文档。

## 参考

- [PassXYZ.Vault2 移动应用](https://github.com/passxyz/PassXYZ.Vault2)

