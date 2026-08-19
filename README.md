# MovieApi

MovieApi is an educational ASP.NET Core Web API for working with movies, actors, genres, reviews, relationships, reporting, authentication, and API security.

The API is used by the separate React frontend project:

[React TypeScript Movie App](https://github.com/wach78/lexicon-react-ts-movie)

## Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- JWT authentication
- ASP.NET Core Antiforgery
- ASP.NET Core Rate Limiting
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
- Entity Framework Core migrations and seed data

## Authentication and Security

The project contains a JWT authentication flow created as a learning exercise.

Current security features include:

- JWT access tokens
- Refresh tokens with rotation
- Access token stored in a `HttpOnly`, `Secure`, `SameSite=Strict` cookie
- Refresh token stored in a `HttpOnly`, `Secure`, `SameSite=Strict` cookie
- ASP.NET Core Antiforgery protection with the `X-CSRF-TOKEN` header
- CSRF protection on login, refresh, and logout
- Protected endpoints using `[Authorize]`
- Login rate limiting per IP address
- CORS configured for the HTTPS React development frontend

The access token currently expires after **15 minutes** and the refresh token after **7 days**.

Refresh tokens are stored in an in-memory `ConcurrentDictionary`. This means refresh sessions are lost when the API restarts. This is intentional for the current educational exercise and would normally be replaced by persistent or distributed storage in a production application.

The current login user is also hardcoded for learning purposes and is not intended as a production authentication implementation.

## Authentication Endpoints

The authentication controller provides endpoints for:

- `GET /api/auth/csrf` - creates and returns an antiforgery request token
- `POST /api/auth/login` - authenticates the demo user and creates authentication cookies
- `GET /api/auth/me` - checks the currently authenticated user
- `POST /api/auth/refresh` - rotates the refresh token and creates new authentication cookies
- `POST /api/auth/logout` - removes the refresh token and authentication cookies

## CSRF Flow

The frontend first requests a CSRF token from:

```text
GET /api/auth/csrf
```

For protected state-changing authentication requests, the request token is then sent in the header:

```text
X-CSRF-TOKEN: <token>
```

The antiforgery cookie is `HttpOnly`, so the frontend does not read it directly. The browser sends it automatically with credentialed requests.

## Rate Limiting

Login uses a fixed-window rate limit:

```text
5 requests per IP address per minute
```

Requests above the limit receive:

```text
429 Too Many Requests
```

## Local Development

Requirements:

- .NET 10 SDK
- SQL Server LocalDB or another configured SQL Server instance

Restore and run the API:

```bash
dotnet restore
dotnet run
```

The default connection string uses SQL Server LocalDB.

The React frontend currently connects to:

```text
https://localhost:7030/api
```

CORS is configured for the HTTPS Vite development origins on ports `5173` and `5174`.

## JWT Configuration

JWT configuration uses:

```text
Jwt:Key
Jwt:Issuer
Jwt:Audience
```

For a real application, the signing key should not be committed to source control.

For local development, .NET User Secrets can be used instead:

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "<strong-random-secret>"
```

Production systems should use an appropriate secret-management solution.

## API Documentation

OpenAPI and Scalar API reference are enabled in the Development environment.

## Purpose

This project is primarily intended for learning and practicing:

- ASP.NET Core Web API development
- Entity Framework Core
- DTOs and service-based application structure
- Relationships between entities
- JWT authentication
- Refresh-token rotation
- Secure cookies
- CSRF protection
- Rate limiting
- CORS
- API security concepts

## Note

This is an educational project and not a production-ready authentication system.
