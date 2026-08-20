# MovieApi

MovieApi is an educational ASP.NET Core Web API for working with movies, actors, genres, reviews, relationships, reporting, authentication, authorization, and API security.

The API is used by the separate React TypeScript frontend project:

[React TypeScript Movie App](https://github.com/wach78/lexicon-react-ts-movie)

## Technologies

- .NET 10
- ASP.NET Core Web API
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server / LocalDB
- JWT authentication
- Refresh tokens
- ASP.NET Core Antiforgery
- ASP.NET Core Rate Limiting
- Role-based authorization
- OpenAPI
- Scalar API reference

## Features

The API currently supports:

- Movie CRUD operations
- Movie details
- Actors
- Reviews
- Movie and actor relationships
- Filtering and searching movies
- Reporting endpoints
- DTO-based API models
- Entity Framework Core migrations
- Movie seed data
- Identity user and role seed data
- User authentication with email and password
- Role-based authorization
- JWT access tokens
- Refresh-token rotation
- Secure authentication cookies
- CSRF protection
- Login rate limiting

## ASP.NET Core Identity

ASP.NET Core Identity is used for user and role management.

The application uses a custom Identity user:

```text
ApplicationUser : IdentityUser<Guid>
```

`ApplicationUser` adds the following properties:

```text
Status
CreatedAt
UpdatedAt
```

The user ID uses `Guid`.

Email is used as the login identifier and unique email addresses are required.

Identity handles:

- User storage
- Password hashing and verification
- User validation
- Roles
- User-role relationships
- Security stamps
- Lockout-related fields
- Identity token providers

The application uses `UserManager<ApplicationUser>` and `RoleManager<IdentityRole<Guid>>` through the authentication service.

## Identity Database Tables

The default ASP.NET Identity table names have been replaced with shorter names:

```text
Users
Roles
UserRoles
UserClaims
UserLogins
RoleClaims
UserTokens
```

`NormalizedEmail` has a unique database index.

## Seeded Users

The development seed creates two users.

### Administrator

```text
Email: admin@example.com
Password: Admin123!
Roles:
- Admin
- User
```

### Standard User

```text
Email: user@example.com
Password: User123!
Role:
- User
```

These accounts are intended for development and learning only.

## Authentication Architecture

Authentication is separated into several services:

```text
AuthController
    |
    +-- IAuthService
    |      |
    |      +-- ASP.NET Core Identity
    |      +-- UserManager<ApplicationUser>
    |
    +-- ITokenService
    |      |
    |      +-- JWT access tokens
    |      +-- Refresh-token generation
    |
    +-- IAuthCookieService
           |
           +-- Access-token cookie
           +-- Refresh-token cookie
           +-- Cookie deletion
```

### AuthService

`AuthService` is responsible for Identity-related authentication operations such as:

- Finding users by email
- Verifying passwords
- Checking user status
- Finding active users by ID
- Retrieving user roles

### TokenService

`TokenService` is responsible for:

- Generating JWT access tokens
- Adding user ID, email, and role claims
- Generating cryptographically random refresh tokens

### AuthCookieService

`AuthCookieService` is responsible for:

- Creating the access-token cookie
- Creating the refresh-token cookie
- Removing authentication cookies

This keeps HTTP cookie handling separate from Identity and token-generation logic.

## JWT Authentication

After successful Identity authentication, the API creates a JWT access token.

The token contains claims for:

```text
NameIdentifier
Name
Email
Role
```

A user can have multiple role claims.

For example, the seeded administrator has:

```text
Admin
User
```

while the standard user only has:

```text
User
```

The access token is stored in an HTTP-only cookie.

## Authentication Cookies

### Access Token

The JWT access token is stored using:

```text
HttpOnly = true
Secure = true
SameSite = Strict
Path = /
```

### Refresh Token

The refresh token is stored using:

```text
HttpOnly = true
Secure = true
SameSite = Strict
Path = /api/auth
```

JavaScript in the frontend does not directly read either authentication token.

The browser sends the cookies automatically with credentialed requests.

## Refresh Tokens

Refresh tokens are rotated when the access token is renewed.

The current refresh-token implementation stores refresh sessions in an in-memory:

```text
ConcurrentDictionary
```

Each refresh token is associated with:

```text
UserId
Expiration time
```

During refresh, the API:

1. Reads the refresh-token cookie.
2. Verifies that the token exists.
3. Checks its expiration.
4. Loads the Identity user by ID.
5. Verifies that the user is still active.
6. Loads the user's current roles.
7. Removes the old refresh token.
8. Generates a new JWT access token.
9. Generates a new refresh token.
10. Replaces the authentication cookies.

Because refresh tokens are currently stored in memory, all refresh sessions are lost when the API restarts.

Persistent or distributed refresh-token storage would normally be used in a production system.

## Authentication Endpoints

### Get CSRF Token

```http
GET /api/auth/csrf
```

Creates an ASP.NET Core antiforgery token and returns the request token to the frontend.

### Login

```http
POST /api/auth/login
```

Example request:

```json
{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

The endpoint:

1. Validates the CSRF token.
2. Finds the Identity user by email.
3. Verifies the password through ASP.NET Core Identity.
4. Checks that the user account is active.
5. Loads the user's roles.
6. Generates an access token.
7. Generates a refresh token.
8. Stores both tokens in secure HTTP-only cookies.

Successful login returns:

```text
204 No Content
```

### Current User

```http
GET /api/auth/me
```

Requires authentication.

Returns information from the authenticated JWT claims, including:

```text
UserId
Email
Roles
```

### Refresh Session

```http
POST /api/auth/refresh
```

Validates the refresh token and rotates both the access token and refresh token.

The user's current Identity status and roles are checked again when the access token is refreshed.

### Logout

```http
POST /api/auth/logout
```

Removes the current refresh token and deletes the authentication cookies.

## CSRF Protection

Because authentication uses cookies, state-changing authentication requests are protected against Cross-Site Request Forgery.

ASP.NET Core Antiforgery is configured with:

```text
Header: X-CSRF-TOKEN
Cookie: X-CSRF-COOKIE
```

The antiforgery cookie uses:

```text
HttpOnly = true
Secure = true
SameSite = Strict
```

The frontend first requests a CSRF token:

```http
GET /api/auth/csrf
```

The returned request token is then sent with state-changing requests:

```http
X-CSRF-TOKEN: <token>
```

CSRF validation is currently used on:

```text
POST /api/auth/login
POST /api/auth/refresh
POST /api/auth/logout
```

## Rate Limiting

Login uses a fixed-window rate limiter.

Current limit:

```text
5 login requests
per IP address
per minute
```

Requests above the limit receive:

```text
429 Too Many Requests
```

## Authorization

ASP.NET Core authorization is enabled.

Endpoints can require an authenticated user:

```csharp
[Authorize]
```

Role-based authorization can also be used:

```csharp
[Authorize(Roles = "Admin")]
```

or:

```csharp
[Authorize(Roles = "Admin,User")]
```

Roles are loaded from ASP.NET Core Identity and added to the JWT as role claims.

## CORS

The API allows credentialed requests from the HTTPS React development frontend.

Current development origins:

```text
https://localhost:5173
https://localhost:5174
```

CORS allows:

- Credentials
- HTTP methods
- Request headers

## Local Development

### Requirements

- .NET 10 SDK
- SQL Server LocalDB or another configured SQL Server instance

Restore dependencies:

```bash
dotnet restore
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the API:

```bash
dotnet run
```

The React frontend currently connects to:

```text
https://localhost:7030/api
```

## Database Migrations

Entity Framework Core migrations are used for both the movie domain and ASP.NET Core Identity schema.

Create a new migration:

```bash
dotnet ef migrations add MigrationName
```

Apply migrations:

```bash
dotnet ef database update
```

## JWT Configuration

JWT configuration uses:

```text
Jwt:Key
Jwt:Issuer
Jwt:Audience
```

The signing key should not be committed to source control in a production application.

For local development, .NET User Secrets can be used:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<strong-random-secret>"
```

Production environments should use an appropriate secret-management solution.

## API Documentation

OpenAPI and Scalar API reference are enabled in the Development environment.

## Security Overview

The project currently demonstrates:

- ASP.NET Core Identity
- Password hashing through Identity
- Unique email accounts
- User account status
- Role management
- JWT authentication
- Role claims
- HttpOnly cookies
- Secure cookies
- SameSite cookies
- Refresh-token rotation
- CSRF protection
- Login rate limiting
- CORS with credentials
- Authentication and authorization separation

## Purpose

This project is primarily intended for learning and practicing:

- ASP.NET Core Web API development
- Entity Framework Core
- ASP.NET Core Identity
- User and role management
- DTOs
- Service-based architecture
- Dependency injection
- JWT authentication
- Refresh tokens
- Authentication cookies
- CSRF protection
- Role-based authorization
- Rate limiting
- CORS
- API security concepts

## Production Considerations

This is an educational project and is not intended to represent a complete production authentication system.

A production implementation should additionally consider:

- Persistent refresh-token storage
- Refresh-token revocation
- Logout from all devices
- Account confirmation flows
- Password reset flows
- Multi-factor authentication
- Strong secret management
- Security auditing and logging
- More advanced account lockout policies
- Production CORS configuration
- HTTPS certificate management
