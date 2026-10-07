# TaskFlow Backend

REST API for **TaskFlow**, a task-management application with JWT authentication, email verification via OTP, and role-based access for admins and members.

Built with **ASP.NET Core 9** using a layered architecture (Domain, Application, Infrastructure, Presentation, Host).

<p align="center">
  <img src="https://skillicons.dev/icons?i=cs,dotnet&theme=light" alt="Core technologies" />
</p>

<p align="center">
  <img alt="ASP.NET Core 9" src="https://img.shields.io/badge/ASP.NET_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img alt="Entity Framework Core" src="https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img alt="SQL Server" src="https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" />
  <img alt="JWT" src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" />
  <img alt="OpenAPI" src="https://img.shields.io/badge/OpenAPI-6BA539?style=for-the-badge&logo=openapiinitiative&logoColor=white" />
  <img alt="Scalar" src="https://img.shields.io/badge/Scalar-1B1B1F?style=for-the-badge&logo=swagger&logoColor=white" />
</p>

## Features

- **Authentication** — Register, email verification (OTP), login, logout, change password, password reset
- **Tasks** — Create, list, get by id, delete, and reassign tasks
- **Users** — Admin-only user listing, lookup, and deletion
- **Security** — JWT bearer tokens, password hashing with salt, token blocklist on logout (in-memory cache)
- **API docs** — OpenAPI + [Scalar](https://github.com/scalar/scalar) UI in Development

## Tech stack

| Layer | Technologies |
|--------|----------------|
| Runtime | .NET 9 |
| Web | ASP.NET Core Web API |
| Data | Entity Framework Core, SQL Server |
| Auth | JWT Bearer |
| Docs | Microsoft.AspNetCore.OpenApi, Scalar.AspNetCore |
| Config | `appsettings.json`, `.env` ([DotNetEnv](https://www.nuget.org/packages/DotNetEnv)) |

## Solution structure

```
TaskFlow-Backend/
├── TaskFlow.Domain/          # Entities, enums, repository interfaces
├── TaskFlow.Application/     # DTOs, application exceptions, service interfaces
├── TaskFlow.Infrastructure/  # EF Core, repositories, JWT/cache services, use cases
├── TaskFlow.Presentation/    # Controllers, view models, global exception middleware
└── TaskFlow.Host/            # Entry point, migrations, appsettings
```

**Dependency flow:** `Host` → `Presentation` → `Infrastructure` → `Application` → `Domain`

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full instance)
- (Optional) [EF Core CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) for migrations

## Configuration

Sensitive settings are not committed. Create a `.env` file in the repository root (see `.gitignore`). Values are loaded at startup via `DotNetEnv.Env.Load()` and merged with ASP.NET Core configuration.

Example `.env`:

```env
ConnectionStrings__DefaultConnection=Server=localhost;Database=TaskFlow;Trusted_Connection=True;TrustServerCertificate=True
JwtSettings__SecretKey=your-long-random-secret-key-at-least-32-characters
```

Optional JWT lifetime overrides (defaults are in `JwtSettings`):

```env
JwtSettings__AccessTokenLifetimeHours=8
JwtSettings__RememberMeLifetimeDays=30
```

Non-secret JWT issuer/audience and validation flags live in `TaskFlow.Host/appsettings.json`.

### OTP during development

Verification and password-reset OTPs are stored in memory and **logged to the console** (no SMTP integration yet). After register or password-reset request, check application logs for the OTP value.

## Getting started

1. Clone the repository and open the solution:

   ```bash
   git clone <repository-url>
   cd TaskFlow-Backend
   ```

2. Add `.env` with connection string and JWT secret (see above).

3. Apply database migrations:

   ```bash
   dotnet ef database update --project TaskFlow.Host
   ```

   If the EF tools are not installed:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. Run the API:

   ```bash
   dotnet run --project TaskFlow.Host
   ```

   Default URLs (from `launchSettings.json`):

   - HTTP: `http://localhost:5051`
   - HTTPS: `https://localhost:7035`

5. In **Development**, open the Scalar API reference (typically `/scalar/v1` on the same base URL) or fetch OpenAPI at `/openapi/v1.json`.

## Authentication

Protected endpoints require the header:

```http
Authorization: Bearer <access_token>
```

Obtain a token from `POST /Auth/login` after verifying email with `POST /Auth/verify`.

## Roles and authorization

| Role | Description |
|------|-------------|
| **Member** | Default on registration. Can manage own assigned tasks (create self-assigned, view own tasks). |
| **Admin** | Can assign/reassign tasks to any user, view all tasks, delete tasks, and manage users. |

Admins must be assigned in the database (`UserRole.Admin`); registration always creates **Member** users.

## API overview

### Auth (`/Auth`)

| Method | Route | Auth | Description |
|--------|--------|------|-------------|
| POST | `/Auth/register` | No | Register; sends verification OTP (logged) |
| POST | `/Auth/verify` | No | Verify email with OTP |
| POST | `/Auth/login` | No | Login; returns JWT |
| POST | `/Auth/logout` | Yes | Invalidate current token |
| POST | `/Auth/change-password` | Yes | Change password |
| POST | `/Auth/request-password-reset` | No | Request reset OTP (logged) |
| POST | `/Auth/reset-password` | No | Reset password with OTP |
| POST | `/Auth/resend-verification-otp` | No | Resend verification OTP |

### Tasks (`/Task`) — all routes require JWT

| Method | Route | Notes |
|--------|--------|--------|
| POST | `/Task/create` | Admins must supply `assignedUserId`; members create tasks assigned to themselves |
| GET | `/Task/{taskId}` | Members: own tasks only; admins: any task |
| GET | `/Task/get-all` | Members: own tasks; admins: all tasks |
| DELETE | `/Task/{taskId}` | Admin only |
| PATCH | `/Task/reassign` | Admin only |

### Users (`/User`) — all routes require JWT, **Admin only**

| Method | Route | Description |
|--------|--------|-------------|
| GET | `/User/{userId}` | Get user by id |
| GET | `/User/get-all` | List all users |
| DELETE | `/User/{userId}` | Delete user |

## Error handling

Unhandled exceptions are mapped to JSON responses by `GlobalExceptionHandler`, including HTTP status codes for validation, not found, conflict, unauthorized, forbidden, and access denied cases.

## Build

```bash
dotnet build TaskFlow.sln
```

## License

Add a license file or section here if this project is distributed under a specific license.
