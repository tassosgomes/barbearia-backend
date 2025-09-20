# Infrastructure Foundation

## Containers
- `api`: ASP.NET Core 8 application with Serilog and OpenTelemetry enabled.
- `sqlserver`: Microsoft SQL Server Developer edition used for local development data storage.
- `otel-collector`: OpenTelemetry Collector routing traces and metrics to the console exporter.

The composition is defined in `docker-compose.yml`. Use `scripts/start-dev.sh` to launch and `scripts/stop-dev.sh` to dispose of the stack.

## Environment Variables
- `ConnectionStrings__BarbeariaDatabase`: SQL Server connection string. Defaults to `Server=localhost,1433;Database=Barbearia;User Id=sa;Password=Your_password123;Encrypt=False;TrustServerCertificate=True` for local containers.
- `HealthChecks__ApiKey`: API key required by `/healthz` endpoint. The compose file falls back to `local-healthcheck-key` when not provided.
- `OTEL_EXPORTER_OTLP_ENDPOINT`: OTLP endpoint for telemetry export. Defaults to `http://otel-collector:4317` inside Docker.
- `ASPNETCORE_ENVIRONMENT`: Hosting environment, defaults to `Development` in local execution.

Configure secrets through environment variables or user secrets. Do not commit production values to the repository.
