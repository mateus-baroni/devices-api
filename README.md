# Devices API

## Overview

Devices API is a RESTful service for managing device resources.

The solution implements all functional requirements from the assignment, including:

- Device creation, retrieval, update, and deletion
- Filtering by brand and state
- Full (PUT) and partial (PATCH) updates
- Soft deletion support
- Automated Entity Framework Core migrations
- Health checks
- Automated unit and integration testing

The solution was implemented using ASP.NET Core, Entity Framework Core, PostgreSQL, and Docker. It follows a layered architecture and enforces all business rules defined in the assignment.

---

## Technology Stack

- .NET 10
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Docker
- Swagger / OpenAPI
- xUnit
- Testcontainers

---

## Running the Application

> The application automatically applies Entity Framework Core migrations on startup.
> No manual migration step is required.

### Option 1 - Run with Docker (Recommended)

#### Prerequisites

- Docker
- Docker Compose

#### Start the application

```bash
docker compose up --build
```

The application will:

- Start PostgreSQL
- Wait for the database to become healthy
- Apply pending EF Core migrations automatically
- Start the API

The API will be available at:

```text
http://localhost:8080
```

### Option 2 - Run Locally

#### Prerequisites

- .NET SDK 10
- PostgreSQL

#### Configure the connection string

Update `src/Devices.Api/appsettings.Development.json` or provide the connection string through environment variables.

Example:

```json
{
  "ConnectionStrings": {
    "DevicesDb": "Host=localhost;Port=5432;Database=devicesdb;Username=postgres;Password=postgres"
  }
}
```

#### Start PostgreSQL

```bash
docker compose up -d postgres
```

#### Run the API

```bash
dotnet run --project src/Devices.Api
```

The API will be available at:

```text
https://localhost:<port>
```

---

## API Documentation

Swagger/OpenAPI documentation is available at:

### Docker

```text
http://localhost:8080/swagger
```

### Local Execution

```text
https://localhost:<port>/swagger
```

---

## API Conventions

### Device State

Supported values:

```text
Available
InUse
Inactive
```

### Error Handling

The API uses RFC 7807 Problem Details responses for validation and business-rule errors.

---

## Health Check

A health endpoint is available for monitoring and container orchestration.

### Docker

```text
http://localhost:8080/health
```

### Local Execution

```text
https://localhost:<port>/health
```

---

## Running Tests

Execute all tests:

```bash
dotnet test
```

The integration tests use Testcontainers and automatically provision an isolated PostgreSQL instance during execution.

### Test Coverage

The test suite contains:

- Unit tests for domain behavior and business rules.
- Integration tests against a real PostgreSQL database using Testcontainers.
- API endpoint tests covering success, validation, conflict, and not-found scenarios.

Coverage reports can be generated with:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

and visualized with ReportGenerator.

---

## Features

The API supports the following operations:

| Method | Endpoint | Description |
|----------|----------|----------|
| POST | `/api/devices` | Create a device |
| GET | `/api/devices/{id}` | Retrieve a device by id |
| GET | `/api/devices` | Retrieve devices with optional filters |
| PUT | `/api/devices/{id}` | Fully update a device |
| PATCH | `/api/devices/{id}` | Partially update a device |
| DELETE | `/api/devices/{id}` | Delete a device |

### Supported Filters

Retrieve all devices:

```http
GET /api/devices
```

Filter by brand:

```http
GET /api/devices?brand=Apple
```

Filter by state:

```http
GET /api/devices?state=Available
```

Combine filters:

```http
GET /api/devices?brand=Apple&state=Available
```

---

## Business Rules

The following domain rules are enforced:

- Device creation date cannot be modified.
- Name and Brand cannot be changed when the device is in use.
- Devices in use cannot be deleted.
- Device deletion is implemented as a soft delete.
- Soft-deleted devices are excluded from all API queries and updates.

---

## Solution Structure

The solution is organized into four projects:

```text
src/
├── Devices.Api
├── Devices.Application
├── Devices.Domain
└── Devices.Infrastructure
```

### Devices.Api

Contains:

- Controllers
- API configuration
- Swagger/OpenAPI configuration

### Devices.Application

Contains:

- Application services
- Request/response DTOs
- Application orchestration logic

### Devices.Domain

Contains:

- Domain entities
- Business rules
- Domain exceptions

### Devices.Infrastructure

Contains:

- Entity Framework Core configuration
- PostgreSQL persistence
- Migrations

---

## Design Decisions

### Soft Delete

The solution uses soft deletion through the `DeletedAt` column.

Deleted devices:

- Are not returned by API queries.
- Cannot be updated.
- Remain available in the database for operational troubleshooting and auditing purposes.

### UpdatedAt Tracking

The `UpdatedAt` field is automatically maintained by the domain model.

It is only updated when a meaningful change occurs to the entity.

### Filtering Through Query Parameters

Filtering by brand and state is implemented using query parameters:

```http
GET /api/devices?brand=Apple&state=Available
```

This approach allows future support for pagination and sorting without introducing additional endpoints.

### Read Query Optimization

Read operations use Entity Framework Core's `AsNoTracking()` to avoid unnecessary change tracking and improve query performance.

Database indexes are defined for:

- Brand
- State

to improve filtering performance.

---

## Assumptions

- Device names are not required to be unique.
- Soft-deleted devices are not recoverable through the public API.
- Filtering currently performs exact matches.
- The API uses UTC timestamps for all date fields.

---

## Future Improvements

- Pagination and sorting support.
- FluentValidation for request validation.
- Authentication and authorization.
- OpenTelemetry observability and distributed tracing.
- Administrative APIs for querying and restoring soft-deleted devices.
- Response caching for read-heavy workloads.
- Rate limiting and API throttling.

---

## Commit History

The repository contains incremental commits demonstrating the evolution of the solution and implementation decisions throughout the development process.