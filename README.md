# CursosOnline

A web application for managing online courses, built with ASP.NET Core 8 and PostgreSQL.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Repository

[https://github.com/githubegg12/Plataforma-de-Cursos-Online.git](https://github.com/githubegg12/Plataforma-de-Cursos-Online.git)

## Getting Started

### 1. Database Configuration

The project uses PostgreSQL. The easiest way to run the database is using Docker Compose, which sets up a PostgreSQL container with the following default configuration:

- **Host**: `localhost` (mapped to port `5433` to avoid conflicts)
- **Database**: `cursosonline`
- **User**: `postgres`
- **Password**: `postgres`

These settings are configured in `docker-compose.yml`.

### 2. Running the Application

You can run the entire application (API/Frontend + Database) using Docker Compose.

1. Open a terminal in the root directory of the project.
2. Run the following command:

   ```bash
   docker compose up --build
   ```

3. The application will be accessible at [http://localhost:5055](http://localhost:5055).

### 3. Database Migrations

Migrations are applied **automatically** when the application starts.

If you need to run migrations manually during development (without Docker):

1. Ensure your `appsettings.json` points to a running PostgreSQL instance.
2. Run:

   ```bash
   dotnet ef database update --project CursosOnline.Infrastructure --startup-project CursosOnline.Web
   ```

### 4. Test Credentials

The application is seeded with a default administrator account:

- **Email**: `admin@cursosonline.com`
- **Password**: `Admin123!`

Use these credentials to log in at [http://localhost:5055/Auth/Login](http://localhost:5055/Auth/Login).

## Project Structure

- **CursosOnline.Web**: The ASP.NET Core MVC application (Frontend & API).
- **CursosOnline.Application**: Business logic and services.
- **CursosOnline.Domain**: Entities and interfaces.
- **CursosOnline.Infrastructure**: Data access and repositories.
- **CursosOnline.Identity**: Identity and authentication logic.

## Deployment (Clever Cloud)

When deploying to Clever Cloud (or any production environment), you must configure the following **Environment Variables** in the Clever Cloud dashboard:

### Required Environment Variables

| Variable Name | Description | Example Value |
|--------------|-------------|---------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL Connection String | `Host=...;Port=...;Database=...;Username=...;Password=...` |
| `JwtSettings__Key` | Secret key for JWT generation | `YourSuperSecretKey...` |
| `JwtSettings__Issuer` | JWT Issuer | `CursosOnline` |
| `JwtSettings__Audience` | JWT Audience | `CursosOnlineUser` |
| `ASPNETCORE_ENVIRONMENT` | Environment setting | `Production` |
| `ASPNETCORE_HTTP_PORTS` | Port for the application | `8080` (Clever Cloud expects the app to listen on 8080) |

> [!NOTE]
> **Database Migrations**: The application is configured to automatically apply pending database migrations when it starts up. You do **not** need to run migrations manually in production.

## Author

- **David Felipe Vargas Varela**
- **ID**: 1140893306
- **Clan**: Caiman
- **Email**: davidvargas1224@gmail.com

