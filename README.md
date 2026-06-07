# Order Management API

Comprehensive documentation for the Order Management API — an ASP.NET Core Web API built with Clean Architecture, Entity Framework Core, SQL Server, Serilog, Health Checks, and OpenTelemetry. This README is intended for onboarding, interviews, and reference.

**Project Overview**
- **Purpose:** Provide a simple, maintainable service to manage orders (CRUD) for an e-commerce-style domain. It demonstrates Clean Architecture patterns, observability, structured logging, and testability.

**Key Features**
- **CRUD** for `Order` entities.
- Clean Architecture separation (API, Application, Domain, Infrastructure).
- Persistence with Entity Framework Core and SQL Server.
- Structured logging with Serilog (console + file).
- Health checks (database) and readiness endpoint.
- Observability via OpenTelemetry (traces, metrics).
- API documentation with Swagger/OpenAPI.
- Unit tests with xUnit, Moq, FluentAssertions.

**Business Flow**
- Client calls API endpoints to create, read, update, and delete orders.
- `OrdersController` calls the `IOrderService` in the Application layer.
- `OrderService` enforces business rules, uses `IOrderRepository` to persist via EF Core in Infrastructure.
- Observability and logging capture requests, errors, and performance data.

**Architecture**
- **Clean Architecture explanation:** The solution separates responsibilities into distinct projects:
  - `API` (presentation) — HTTP controllers, middleware, and DI wiring.
  - `Application` — business use cases, DTOs, service interfaces.
  - `Domain` — core entities and domain logic (POCOs).
  - `Infrastructure` — EF Core DbContext, repository implementations, migrations.
  - `Tests` — unit tests against services, controllers, and repositories.

**Project Structure**
- **API:** [API/](API/) — HTTP layer, `OrdersController`, middleware, Program entrypoint.
- **Application:** [Application/](Application/) — `IOrderService`, `OrderService`, DTOs and interfaces.
- **Domain:** [Domain/](Domain/) — `Order` entity and domain models.
- **Infrastructure:** [Infrastructure/](Infrastructure/) — `AppDbContext`, `SqlOrderRepository`, EF Migrations.
- **Tests:** [Tests/](Tests/) — xUnit tests: controller, service, repository tests.

**Dependency Flow Between Layers**
- Direction: API -> Application -> Domain -> Infrastructure.
- Only Application depends on Domain types; Infrastructure implements Application interfaces.

**Technologies Used**
- **ASP.NET Core:** HTTP API hosting, routing, middleware.
- **Entity Framework Core:** ORM and migrations.
- **SQL Server:** Primary relational database for persistence.
- **OpenTelemetry:** Tracing and metrics collection.
- **Serilog:** Structured logging to console and file sinks.
- **Health Checks:** ASP.NET Core health checks for readiness/liveness.
- **Swagger/OpenAPI:** Automatically generated API docs via Swashbuckle.
- **xUnit:** Unit testing framework.
- **Moq:** Mocking dependencies in unit tests.
- **FluentAssertions:** Fluent assertions in tests.

**Design Patterns Used**
- **Repository Pattern:** Abstracts persistence with `IOrderRepository` and `SqlOrderRepository`.
- **Dependency Injection:** All services and repositories registered in DI container.
- **Service Layer Pattern:** `OrderService` encapsulates business use-cases.
- **Middleware Pattern:** `ExceptionMiddleware` for centralized error handling and response shaping.
- **SOLID Principles implementation:** Single Responsibility across layers, Interface segregation (`IOrderRepository`, `IOrderService`), Dependency inversion (higher-level modules depend on abstractions), etc.

**Database Design**
- **Order entity (core fields):**
  - `Id` (GUID or int) — Primary key.
  - `OrderNumber` (string) — Unique business identifier.
  - `CustomerName` (string)
  - `CustomerEmail` (string)
  - `Total` (decimal)
  - `Status` (string) — e.g., Pending, Confirmed, Shipped, Cancelled.
  - `CreatedAt` (DateTimeOffset)
  - `UpdatedAt` (DateTimeOffset?)

**Database Schema**
- Table: `Orders`
  - `Id` INT IDENTITY(1,1) PRIMARY KEY (or `uniqueidentifier` for GUIDs)
  - `OrderNumber` NVARCHAR(50) NOT NULL
  - `CustomerName` NVARCHAR(200) NOT NULL
  - `CustomerEmail` NVARCHAR(200) NULL
  - `Total` DECIMAL(18,2) NOT NULL
  - `Status` NVARCHAR(50) NOT NULL
  - `CreatedAt` DATETIMEOFFSET NOT NULL
  - `UpdatedAt` DATETIMEOFFSET NULL

**Relationships**
- This initial model is single-entity (Orders). If extended, relationships might include `OrderItems` (1:N), `Customer` (N:1), `ShippingAddress` (1:1).

**Migrations Commands**
- Add migration (run from solution root):

```
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
```

- Apply migrations / update database:

```
dotnet ef database update --project Infrastructure --startup-project API
```

**API Endpoints**
- GET `/api/orders` — List orders (paging/filtering can be added).
- GET `/api/orders/{id}` — Get single order by id.
- POST `/api/orders` — Create a new order.
- PUT `/api/orders/{id}` — Update existing order.
- DELETE `/api/orders/{id}` — Delete order by id.

**Sample Request and Response Payloads**
- POST /api/orders (request):

```json
{
  "orderNumber": "ORD-1001",
  "customerName": "Jane Doe",
  "customerEmail": "jane.doe@example.com",
  "total": 123.45,
  "status": "Pending"
}
```

- POST /api/orders (response: 201 Created):

```json
{
  "id": 1,
  "orderNumber": "ORD-1001",
  "customerName": "Jane Doe",
  "customerEmail": "jane.doe@example.com",
  "total": 123.45,
  "status": "Pending",
  "createdAt": "2026-06-07T12:34:56Z"
}
```

- GET /api/orders/{id} (response 200 OK): same as above.

**Status Codes**
- 200 OK — Successful GET/PUT/DELETE where appropriate.
- 201 Created — Successful POST.
- 400 Bad Request — Validation errors.
- 404 Not Found — Resource missing.
- 500 Internal Server Error — Unexpected failures.

**Exception Handling**
- `ExceptionMiddleware` centralizes exception handling, logs exceptions, and returns consistent error responses.

**Error Response Format**
- Standard error response JSON:

```json
{
  "traceId": "00-...",
  "status": 400,
  "errors": ["customerName is required"]
}
```

**Logging**
- **Serilog configuration:** Configured in `Program.cs` and `appsettings.json` to write to console and rolling file sinks.
- **Console logging:** Human-friendly output for local development with enrichment (timestamp, level, trace id).
- **File logging:** Rolling logs in `Logs/` with daily files (example: `Logs/log-20260607.txt`).
- **Structured logging examples:**

```
Information: Received POST /api/orders { OrderNumber = ORD-1001, Total = 123.45 }
Error: Exception while processing request { Exception = "System.InvalidOperationException: ...", TraceId = "00-..." }
```

**Health Checks**
- Database health check implemented in [Health/DatabaseHealthCheck.cs](Health/DatabaseHealthCheck.cs).
- Registered via `AddHealthChecks()` in `Program.cs` and exposed at `/health` (or `/health/ready`).

**Health Endpoint Usage**
- Liveness/Readiness sample request:

```
GET /health
```

- Sample response (200 OK):

```json
{
  "status": "Healthy",
  "checks": [
    { "name": "Database", "status": "Healthy", "exception": null }
  ]
}
```

**Observability (OpenTelemetry)**
- **Tracing:** Instrumentation of incoming HTTP requests, EF Core, and custom spans in `OrderService`.
- **Metrics:** Request counts, latencies, and DB call metrics.
- **Logging integration:** Correlate traces with Serilog by including the `traceId` in logs.
- **Example telemetry output (simplified):**

```
Span: GET /api/orders -> duration 25ms
Span: EFCore.Query Orders SELECT ... -> duration 12ms
Metric: http.server.duration { route = "/api/orders", status = 200 } 25
```

**Dependency Injection**
- Service registrations performed in `API/Program.cs` via extension methods. Typical registrations:

```csharp
services.AddScoped<IOrderService, OrderService>();
services.AddScoped<IOrderRepository, SqlOrderRepository>();
services.AddDbContext<AppDbContext>(...);
```

**Repository Registrations**
- `SqlOrderRepository` registered as the implementation for `IOrderRepository` in DI; `AppDbContext` is registered and managed by EF Core.

**Unit Testing**
- **OrdersController tests:** [Tests/Controllers/OrdersControllerTests.cs](Tests/Controllers/OrdersControllerTests.cs) — verify controller behavior and HTTP responses using mocked services.
- **OrderService tests:** [Tests/Services/OrderServiceTests.cs](Tests/Services/OrderServiceTests.cs) — verify business rules using mocked `IOrderRepository`.
- **SqlOrderRepository tests:** [Tests/Repositories/SqlOrderRepositoryTests.cs](Tests/Repositories/SqlOrderRepositoryTests.cs) — integration-style tests using in-memory or test SQL instance.

**Mocking Strategy**
- Use `Moq` to create `Mock<IOrderRepository>` and `Mock<IOrderService>` where appropriate.
- Use `FluentAssertions` for expressive assertions.

**Test Execution Commands**

```
dotnet test Tests/Tests.csproj
```

or run all tests in solution root:

```
dotnet test
```

**Setup Instructions**
- **Prerequisites:**
  - [.NET SDK 8.0+] installed.
  - SQL Server (local or container) accessible.
  - `dotnet-ef` tool installed (optional globally): `dotnet tool install --global dotnet-ef`.

- **Clone repository:**

```
git clone <repo-url>
cd OrderManagementAPI
```

- **Restore packages:**

```
dotnet restore
```

- **Apply migrations:**

```
dotnet ef database update --project Infrastructure --startup-project API
```

- **Run application:**

```
dotnet run --project API
```

**Build and Run Commands**
- `dotnet restore`
- `dotnet build`
- `dotnet ef database update --project Infrastructure --startup-project API`
- `dotnet run --project API`
- `dotnet test`

**Folder Structure Diagram**

```
OrderManagementAPI/
├─ API/
│  ├─ Controllers/OrdersController.cs
│  ├─ Middleware/ExceptionMiddleware.cs
  │  └─ Program.cs
├─ Application/
│  ├─ Services/OrderService.cs
│  └─ Interfaces/IOrderService.cs
├─ Domain/
│  └─ Entities/Order.cs
├─ Infrastructure/
│  ├─ Persistence/AppDbContext.cs
│  └─ Repositories/SqlOrderRepository.cs
└─ Tests/
   └─ (xUnit tests)
```

**Future Enhancements**
- **Authentication and Authorization:** Add JWT or OAuth2 support.
- **Caching:** Add Redis or in-memory caching for read-heavy endpoints.
- **Docker support:** Dockerfile and docker-compose for local development.
- **CI/CD pipeline:** GitHub Actions to build, test, and publish images.
- **OpenTelemetry Collector integration:** Centralize and export traces/metrics.
- **Distributed tracing:** Integrate with Jaeger/Zipkin/Azure Monitor for cross-service traces.

---

If you'd like, I can also:
- add a `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`, or a sample `docker-compose` and `Dockerfile` for local SQL Server;
- generate Postman/HTTP collection for the API endpoints;
- or open a PR with the README committed.

If you want any section expanded (DB ERD, deeper sample traces, or CI YAML), tell me which one to expand.
