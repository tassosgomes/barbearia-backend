# Local Development Setup

## Prerequisites
- Docker Desktop or Docker Engine 20.10+
- .NET SDK 8.0.4+ (required for C# 12 and ASP.NET Core 8)

## Environment Variables
Configure secrets with `dotnet user-secrets` whenever possible.

| Variable | Description | Default | Scope |
| --- | --- | --- | --- |
| `ConnectionStrings__SqlDatabase` | SQL Server connection string used by the API. | `Server=localhost,1433;Database=Barbearia;User Id=sa;Password=YourStrong@Password1;Encrypt=False;TrustServerCertificate=True` | API |
| `Monitoring__HealthCheckKey` | Shared secret required to access `/health`. | `local-development-health-key` | API |
| `Serilog__MinimumLevel__Default` | Minimum logging level override. | `Information` | API |
| `MSSQL_SA_PASSWORD` | SA password for the SQL Server container. | `YourStrong@Password1` | Database |

Example using user-secrets for the API project:

```bash
dotnet user-secrets init --project src/Barbearia.Api
DOTNET_ENVIRONMENT=Development \
dotnet user-secrets set "ConnectionStrings:SqlDatabase" "Server=localhost,1433;Database=Barbearia;User Id=sa;Password=YourStrong@Password1;Encrypt=False;TrustServerCertificate=True" --project src/Barbearia.Api
dotnet user-secrets set "Monitoring:HealthCheckKey" "replace-with-strong-key" --project src/Barbearia.Api
```

## Running with Docker Compose

```bash
docker compose up --build
```

The API listens on `http://localhost:5000`. Access the protected health check with:

```bash
curl http://localhost:5000/health \
  -H "X-Health-Api-Key: local-development-health-key"
```

SQL Server is exposed on port `1433` with a persisted volume named `sqlserver-data`.

## Useful Commands

```bash
# Restore and build the solution
dotnet restore
dotnet build

# Run the API locally without Docker
dotnet run --project src/Barbearia.Api
```
