# PassXYZ Vault 3 Server

The shared backend API service for PassXYZ and Finanalyzer ecosystem applications, providing password vault management, user authentication, and Dashboard/Portfolio services.

## Overview

PassXYZ.Server is a unified backend API service built with ASP.NET Core that serves as the central data layer for:

- **passxyz-web** - Web-based password manager compatible with KeePass
- **finanalyzer-app** - Financial analysis workbench with dashboard functionality

The server handles authentication, KeePass database operations, session management, and provides RESTful APIs for both frontend applications.

**Key Features:**

- Multi-layer authentication (Cloudflare Access + JWT + master password)
- KeePass-compatible password vault operations
- Single sign-on enforcement (prevents concurrent logins)
- Device lock support
- RESTful API endpoints for vault management
- Dashboard and Portfolio management APIs
- Configurable CORS policies
- Swagger API documentation

## Tech Stack

| Component              | Version  | Description                     |
| ---------------------- | -------- | ------------------------------- |
| .NET                   | 10.0     | Cross-platform runtime          |
| ASP.NET Core           | 10.0     | Web API framework               |
| Swashbuckle.AspNetCore | 10.2.3   | Swagger API documentation       |
| PassXYZLib             | 3.0.1    | KeePass functionality extension |
| KPCLib                 | 2.0.4    | KeePassLib .NET Standard        |
| SQLite                 | Built-in | Lightweight database            |

## Project Structure

```
PassXYZ.Server/
├── Controllers/
│   ├── ConfigController.cs    # Configuration file endpoints ✅
│   ├── DashboardController.cs # Dashboard API ✅
│   ├── PortfolioController.cs # Portfolio API ✅
│   ├── UserController.cs      # User management API ✅
│   └── VaultController.cs     # Password vault management API ✅
├── Middleware/
│   ├── CloudflareAccessMiddleware.cs   # Cloudflare Access auth ✅
│   └── JwtAuthenticationMiddleware.cs  # JWT authentication ✅
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
│   └── launchSettings.json    # Launch configuration ✅
├── PassXYZ.Server.csproj      # Project file ✅
├── Program.cs                 # Application entry point ✅
├── appsettings.json           # Application configuration ✅
└── appsettings.Development.json  # Development config ✅
```

**Legend:** ✅ = Implemented, `(planned)` = Not yet implemented

## Getting Started

### Prerequisites

- .NET 10 SDK or later
- Node.js 18+ (for frontend applications)
- pnpm package manager (for frontend applications)

### Installation

```bash
git clone https://github.com/finanalyzer/server.git PassXYZ.Server
# Navigate to the server directory (from monorepo root)
cd PassXYZ.Server

# Restore dependencies
dotnet restore
```

<br />

### Development

```bash
# Start the server
dotnet run
```

### Access

| Service      | URL                                             |
| ------------ | ----------------------------------------------- |
| HTTP         | `http://localhost:5182`                         |
| HTTPS        | `https://localhost:7060`                        |
| Swagger UI   | `http://localhost:5182/swagger`                 |
| OpenAPI JSON | `http://localhost:5182/swagger/v1/swagger.json` |

### Build

```bash
# Build for production
dotnet build --configuration Release

# Build and run
dotnet run --configuration Release
```

## Authentication Architecture

### Multi-layer Security

PassXYZ.Server implements a three-layer authentication system:

1. **Cloudflare Access** - Identity verification (email-based)
2. **Local JWT** - Session authentication
3. **Master Password** - Data decryption (KeePass database)

### Authentication Flow

```
1. User → Cloudflare Access → Cloudflare JWT (contains email)
2. Frontend → POST /api/user/login (Cloudflare JWT + credentials)
3. Backend → Verify Cloudflare JWT → Query user from users.db
4. Check for active sessions → Return 409 Conflict if exists
5. Open KeePass database with master password → Create session
6. Return local JWT → Store in localStorage
7. All subsequent requests → Authorization: Bearer <local-jwt>
```

### Data Flow

```
Frontend Page → REST API → VaultController → VaultService 
                                          → VaultSessionManager → PwDatabase
                                                                      ↓
                                                                UsersDbContext
```

### Session Management

- Token stored in frontend localStorage as `passxyz-token`
- Session timeout: 1 hour (configurable)
- Concurrent login detection: Returns 409 Conflict with session details
- Auto-lock: Frontend triggers `POST /api/user/logout` on timeout
- Device lock: Requires WebAuthn verification for login

### Anonymous Endpoints

The following endpoints are accessible without authentication:

| Endpoint            | Description           |
| ------------------- | --------------------- |
| `/api/user/login`   | User login            |
| `/api/user/signup`  | User registration     |
| `/api/apps.json`    | Apps configuration    |
| `/api/agents.json`  | Agents configuration  |
| `/api/widgets.json` | Widgets configuration |

## API Reference

### User Management (`/api/user`)

| Method | Endpoint               | Description         | Authentication |
| ------ | ---------------------- | ------------------- | -------------- |
| POST   | `/api/user/signup`     | User registration   | Cloudflare JWT |
| POST   | `/api/user/login`      | User login          | Cloudflare JWT |
| POST   | `/api/user/logout`     | User logout         | Local JWT      |
| GET    | `/api/user/profile`    | Get user profile    | Local JWT      |
| PUT    | `/api/user/profile`    | Update user profile | Local JWT      |
| DELETE | `/api/user/{username}` | Delete user         | Local JWT      |
| GET    | `/api/user/list`       | Get user list       | Local JWT      |

#### Login Request

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

#### Login Response (Success)

```json
{
  "token": "local-jwt-token",
  "expiresAt": "2026-07-02T12:00:00Z",
  "username": "alice",
  "email": "alice@example.com"
}
```

#### Login Response (Conflict)

```json
HTTP 409 Conflict

{
  "error": "CONCURRENT_SESSION",
  "message": "Another device is logged in",
  "existingSession": {
    "deviceInfo": "Windows Chrome",
    "loginTime": "2026-07-02T10:00:00Z",
    "ipAddress": "192.168.1.100"
  },
  "confirmUrl": "/api/user/login?takeOver=true"
}
```

### Vault Management (`/api/vault`)

| Method | Endpoint                                   | Description            | Authentication |
| ------ | ------------------------------------------ | ---------------------- | -------------- |
| GET    | `/api/vault/groups/{groupId}/items`        | Get items in group     | Local JWT      |
| GET    | `/api/vault/items/{itemId}`                | Get single item        | Local JWT      |
| GET    | `/api/vault/entries/{entryId}`             | Get entry details      | Local JWT      |
| GET    | `/api/vault/groups/{groupId}`              | Get group details      | Local JWT      |
| GET    | `/api/vault/search?keyword={keyword}`      | Search entries         | Local JWT      |
| GET    | `/api/vault/icons`                         | Get icon list          | Local JWT      |
| POST   | `/api/vault/groups/{groupId}/entries`      | Create entry           | Local JWT      |
| POST   | `/api/vault/groups/{parentGroupId}/groups` | Create group           | Local JWT      |
| PUT    | `/api/vault/entries/{entryId}`             | Update entry           | Local JWT      |
| PUT    | `/api/vault/groups/{groupId}`              | Update group           | Local JWT      |
| DELETE | `/api/vault/entries/{entryId}`             | Delete entry           | Local JWT      |
| DELETE | `/api/vault/groups/{groupId}`              | Delete group           | Local JWT      |
| POST   | `/api/vault/change-password`               | Change master password | Local JWT      |

### Attachment Management

| Method | Endpoint                                                  | Description         |
| ------ | --------------------------------------------------------- | ------------------- |
| GET    | `/api/vault/entries/{entryId}/attachments`                | Get attachment list |
| GET    | `/api/vault/entries/{entryId}/attachments/{attachmentId}` | Download attachment |
| POST   | `/api/vault/entries/{entryId}/attachments`                | Upload attachment   |
| DELETE | `/api/vault/entries/{entryId}/attachments/{attachmentId}` | Delete attachment   |

### Configuration Endpoints (Public)

| Endpoint                | Description               |
| ----------------------- | ------------------------- |
| GET `/api/apps.json`    | Get Apps configuration    |
| GET `/api/agents.json`  | Get Agents configuration  |
| GET `/api/widgets.json` | Get Widgets configuration |

### Dashboard API

| Method | Endpoint                                               | Description           |
| ------ | ------------------------------------------------------ | --------------------- |
| GET    | `/api/v1/dashboard`                                    | Get all dashboards    |
| POST   | `/api/v1/dashboard`                                    | Create new dashboard  |
| GET    | `/api/v1/dashboard/{dashboard_id}`                     | Get single dashboard  |
| PUT    | `/api/v1/dashboard/{dashboard_id}`                     | Update dashboard      |
| DELETE | `/api/v1/dashboard/{dashboard_id}`                     | Delete dashboard      |
| GET    | `/api/v1/dashboard/{dashboard_id}/widgets`             | Get dashboard widgets |
| POST   | `/api/v1/dashboard/{dashboard_id}/widgets`             | Add widget            |
| PUT    | `/api/v1/dashboard/{dashboard_id}/widgets/{widget_id}` | Update widget         |
| DELETE | `/api/v1/dashboard/{dashboard_id}/widgets/{widget_id}` | Delete widget         |

### Portfolio API

| Method | Endpoint                                          | Description        |
| ------ | ------------------------------------------------- | ------------------ |
| GET    | `/api/v1/portfolio/stocks`                        | Get all stocks     |
| POST   | `/api/v1/portfolio/stocks`                        | Add stock          |
| GET    | `/api/v1/portfolio/stocks/{symbol}`               | Get single stock   |
| PUT    | `/api/v1/portfolio/stocks/{symbol}`               | Update stock       |
| DELETE | `/api/v1/portfolio/stocks/{symbol}`               | Delete stock       |
| GET    | `/api/v1/portfolio/transactions`                  | Get transactions   |
| POST   | `/api/v1/portfolio/transactions`                  | Create transaction |
| PUT    | `/api/v1/portfolio/transactions/{transaction_id}` | Update transaction |
| DELETE | `/api/v1/portfolio/transactions/{transaction_id}` | Delete transaction |

## CORS Configuration

PassXYZ.Server supports CORS configuration via `appsettings.json`:

### Default Configuration

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

### Environment Variables

Override configuration via environment variables:

```bash
export Cors__AllowedOrigins__0="http://localhost:5173"
export Cors__AllowedOrigins__1="http://localhost:5174"
export Cors__AllowCredentials="true"
```

### Development Configuration

`appsettings.Development.json` includes additional origins:

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

## Client Integration

### passxyz-web

**Configuration:**

- Base URL: `/api` (via Vite proxy)
- Token Storage: `localStorage` as `passxyz-token`
- Authentication Type: `passxyz-jwt`

**Vite Proxy (`vite.config.ts`):**

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

**Configuration:**

- Base URL: `/api` (via Vite proxy)
- Authentication Type: `passxyz-jwt`
- Token Source: Reads `passxyz-token` from localStorage

**Connection Configuration:**

| Field     | Value          |
| --------- | -------------- |
| Name      | PassXYZ.Server |
| URL       | `/api`         |
| Auth Type | passxyz-jwt    |

**Authentication Flow:**

1. User logs in via passxyz-web
2. JWT token stored in `localStorage` as `passxyz-token`
3. finanalyzer-app reads token from `localStorage`
4. Token automatically added to Authorization header

```typescript
const token = localStorage.getItem('passxyz-token');
if (token) {
  headers['Authorization'] = `Bearer ${token}`;
}
```

**401 Handling:**

When API returns 401, finanalyzer-app clears token and redirects:

```typescript
export function handleUnauthorized(): void {
  localStorage.removeItem('passxyz-token');
  localStorage.removeItem('passxyz-user');
  window.location.href = '/vault/#/login';
}
```

### Local Development Setup

| Application     | Port | Description               |
| --------------- | ---- | ------------------------- |
| passxyz-web     | 5173 | Password manager frontend |
| finanalyzer-app | 5174 | Dashboard application     |
| PassXYZ.Server  | 5182 | Backend API service       |

#### Proxy Integration Mode (Recommended)

All applications share the same origin via passxyz-web proxy:

```
http://localhost:5173/vault/          → passxyz-web
http://localhost:5173/app/            → passxyz-web proxy → finanalyzer-app
http://localhost:5173/api/            → passxyz-web proxy → PassXYZ.Server
```

**Steps:**

1. Start PassXYZ.Server: `cd PassXYZ.Server && dotnet run`
2. Start finanalyzer-app: `cd finanalyzer-app && pnpm run dev`
3. Start passxyz-web: `cd passxyz-web && pnpm run dev`
4. Visit `http://localhost:5173/vault/#/login` to log in
5. Visit `http://localhost:5173/app/` to access dashboard

#### Independent Development Mode

Run each app separately and manually sync tokens:

1. Follow steps 1-4 from Proxy Integration Mode
2. Copy `passxyz-token` from passxyz-web localStorage
3. Set token in finanalyzer-app localStorage via browser console

## Cloudflare Tunnel Setup

### Configuration (`~/.cloudflared/config.yml`)

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

### Start Tunnel

```bash
cloudflared tunnel --no-prechecks run
```

### Access URLs

| Service         | Local URL                      | Tunnel URL                       |
| --------------- | ------------------------------ | -------------------------------- |
| passxyz-web     | `http://localhost:5173/vault/` | `https://yourdomain.com/vault/` |
| finanalyzer-app | `http://localhost:5174/`       | `https://yourdomain.com/app/`   |
| PassXYZ.Server  | `http://localhost:5182/api/`   | `https://yourdomain.com/api/`   |

## Security

- **Password Protection:** HTTPS encryption in transit, KeePass native encryption at rest
- **JWT Security:** Strong key signing with reasonable expiration (1 hour)
- **Session Timeout:** Configurable idle timeout with auto-lock
- **Single Sign-On:** Prevents concurrent logins on multiple devices
- **Input Validation:** ASP.NET Core model validation for all API inputs
- **Device Lock:** WebAuthn standard for biometric verification
- **XSS Protection:** Markdown rendering with HTML tag sanitization
- **OTP Security:** TOTP keys processed client-side only


## Development Testing

### Disable Authentication (Development Only)

Temporarily comment out authentication middleware in `Program.cs`:

```csharp
// app.UseMiddleware<CloudflareAccessMiddleware>();
// app.UseMiddleware<JwtAuthenticationMiddleware>();
```

**Note:** Only for development debugging. Production must enable authentication.

### Swagger Documentation

Access Swagger UI at `http://localhost:5182/swagger` for interactive API documentation.

## References

- [PassXYZ.Vault2 Mobile Application](https://github.com/passxyz/PassXYZ.Vault2)

