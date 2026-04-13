# CSharpModulith

A modular monolith built with .NET 10, ASP.NET Core, and .NET Aspire. Each business capability lives in its own project, enforced by ArchUnitNET architecture tests.

## Solution Structure

```
src/
├── CSharpModulith.Host/                  # ASP.NET Core composition root and HTTP pipeline
├── CSharpModulith.AppHost/               # .NET Aspire orchestration (local dev)
├── CSharpModulith.ServiceDefaults/       # Shared service defaults (OpenTelemetry, health, etc.)
├── CSharpModulith.Shared/                # Shared kernel
├── CSharpModulith.Capability.Order/      # Order capability
├── CSharpModulith.Capability.Payment/    # Payment capability
└── CSharpModulith.Capability.Todos/      # Todos capability

tests/
├── CSharpModulith.Architecture.Tests/    # ArchUnitNET architecture rules
├── CSharpModulith.Capability.Order.Tests/
├── CSharpModulith.Capability.Payment.Tests/
└── CSharpModulith.Capability.Todos.Tests/
```

## Tech Stack

- **.NET 10** / C# (latest)
- **ASP.NET Core** (Host)
- **.NET Aspire** (local orchestration, service defaults)
- **PostgreSQL** (primary database, via Aspire)
- **SQLite** (fallback when PostgreSQL is not configured)
- **RabbitMQ** (messaging)
- **MailPit** (local email testing)
- **Entity Framework Core** (persistence)
- **ArchUnitNET** (architecture tests)
- **xUnit** (test framework)
- **Docker / Docker Compose** (containerized development)

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) and Docker Compose

No local .NET SDK installation is required. All build, test, and run commands execute inside a Docker container.

## Getting Started

Build the dev image and start the stack:

```bash
make up
```

The host is available at **http://localhost:8080**.

## Common Commands

| Command | Description |
|---|---|
| `make up` | Build and start the full stack (detached) |
| `make run` | Build and start in the foreground |
| `make down` | Stop and remove containers |
| `make logs` | Follow container logs |
| `make shell` | Interactive shell in the .NET SDK container |
| `make test` | Run all tests |
| `make build-app` | Build the solution inside the container |
| `make build-dev-image` | Rebuild the dev SDK image |
| `make clean` | Stop containers, remove images and build artifacts |

### Running SDK Commands Directly

```bash
docker compose exec -T csharp dotnet build CSharpModulith.sln -c Release
docker compose exec -T csharp dotnet test CSharpModulith.sln -c Release
docker compose exec -T csharp dotnet format
```

### Aspire AppHost (Local Dev)

For local orchestration with the Aspire dashboard, run the AppHost directly (requires a local .NET 10 SDK):

```bash
dotnet run --project src/CSharpModulith.AppHost
```

This starts PostgreSQL, RabbitMQ, SQLite, and MailPit as Aspire resources alongside the Host.

## Architecture

Each capability is an independent class library project (`CSharpModulith.Capability.<Name>`) following a layered structure:

- **Domain** -- aggregates, entities, value objects, domain events, repository contracts
- **Application** -- use cases, facades, read models, queries, bridge interfaces
- **Infrastructure** -- persistence, messaging, bridge implementations
- **Presentation** -- HTTP endpoints, view models, request/response DTOs

Cross-capability communication goes through **bridges**: an interface declared in the consuming capability's application layer, implemented in its infrastructure layer using the other capability's facade.

Architecture rules are enforced at build time via **ArchUnitNET** tests in `tests/CSharpModulith.Architecture.Tests/`.

## License

Copyright (c) 2026 Florian Krämer. Licensed under the [MIT License](LICENSE).
