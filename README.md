# Task Manager API

A RESTful task management system built with ASP.NET Core following Clean/Onion Architecture. The service is containerized with Docker and orchestrated via Docker Compose, uses PostgreSQL for persistence, AWS Cognito for authentication, Serilog for structured logging, global error handling middleware, pagination metadata, and role-based access control.

## Architecture

```
Presentation   → TaskManager.Api (Controllers, DTOs, Middleware)
Core           → TaskManager.Core (Entities, Interfaces)
Infrastructure → TaskManager.Infrastructure (DbContext, Repositories)
Tests          → TaskManager.Tests (Unit & Integration)
```

## Core Features Implemented

1. **User Authentication** via AWS Cognito (JWT Bearer).
2. **Project Management**: CRUD endpoints under `/api/projects` using `ProjectsController`.
3. **Task Management**: CRUD endpoints under `/api/projects/{projectId}/tasks` using `TasksController`.
4. **Logging**: Structured logging with Serilog and `UseSerilogRequestLogging()` for HTTP requests.
5. **Global Error Handling**: Middleware returning ProblemDetails (`application/problem+json`).
6. **Pagination**: `pageNumber`/`pageSize` parameters, with `X-Total-Count`, `X-Page-Number`, and `X-Page-Size` headers in GET responses.
7. **Role-Based Access Control**: `AdminOnly` policy on delete endpoints.
8. **Unit Tests**: xUnit tests covering `ProjectRepository` and `TaskRepository` with EF Core InMemory.
9. **Integration Tests**: Controller tests with `WebApplicationFactory<Program>`, overriding authentication and using InMemory DbContext.

## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/get-started) & [Docker Compose](https://docs.docker.com/compose/install/)
* AWS Cognito User Pool and App Client (configure `appsettings.json` or environment variables)

### Running Locally with Docker Compose

1. **Clone the repository** and navigate to its root.
2. **Configure** Cognito settings in `TaskManager.Api/appsettings.json` or via environment variables:

   ```json
   "Cognito": {
     "AWSRegion": "<region>",
     "UserPoolId": "<poolId>",
     "ClientId": "<clientId>"
   }
   ```
3. **Start** the database and API services:

   ```bash
     docker-compose up --build
   ```
  ````

4. **Apply migrations** (if not auto-applied):
      dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.Api
 ````

5. **Browse** to Swagger UI: [http://localhost:5000/swagger](http://localhost:5000/swagger)

### Running Tests

```bash
cd TaskManager.Tests
dotnet test
```

## Folder Structure

```
/task-manager
│
├─ TaskManager.Api         # Presentation layer (Controllers, DTOs, Middleware)
│  ├─ Controllers
│  ├─ Models
│  ├─ Middleware
│  ├─ Program.cs
│  └─ appsettings.json
│
├─ TaskManager.Core        # Domain/core entities & repository interfaces
│  └─ Entities
│  └─ Interfaces
│
├─ TaskManager.Infrastructure # Infrastructure layer (EF Core DbContext & Repositories)
│  └─ Data
│  └─ Repositories
│
└─ TaskManager.Tests       # Unit & Integration tests
   ├─ Repositories         # Repository unit tests
   └─ Integration          # Controller integration tests (WebApplicationFactory)
```
