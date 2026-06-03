# Inventory Management System

ASP.NET Core inventory API built as per the requirements given. It manages products, creates orders, updates stock, and applies pricing rules for location, volume, Black Friday, and Polish bank holidays.

## Tech Stack

- ASP.NET Core minimal APIs on `net10.0`
- SQLite persistence with EF Core migrations
- CQRS-style command/query records and handlers
- FluentValidation for request validation
- xUnit unit and integration tests

## Project Layout

```text
src/
  InventoryManagement.Api/             API, domain model, EF Core data access, feature folders
tests/
  InventoryManagement.UnitTests/       pricing/domain unit tests
  InventoryManagement.IntegrationTests/ HTTP API integration tests
```

Feature endpoints are registered through themed extension classes:

- `Products/ProductEndpoints.cs`
- `Orders/OrderEndpoints.cs`

`Program.cs` keeps application startup, DI registration, middleware, database migration/seed, and endpoint registration.

## Running The API

Restore and build:

```powershell
dotnet restore InventoryManagement.sln
dotnet build InventoryManagement.sln
```

Run the API:

```powershell
dotnet run --project src\InventoryManagement.Api\InventoryManagement.Api.csproj
```

Swagger is enabled in development. The included HTTP scratch file is at:

```text
src/InventoryManagement.Api/InventoryManagement.Api.http
```

## Configuration

SQLite is configured through `ConnectionStrings:Inventory`.

Default value:

```json
{
  "ConnectionStrings": {
    "Inventory": "Data Source=inventory.db"
  }
}
```

The app runs EF Core migrations on startup and seeds three customers used for order pricing:

| Region | Customer ID |
| --- | --- |
| United States | `11111111-1111-1111-1111-111111111111` |
| Europe | `22222222-2222-2222-2222-222222222222` |
| Asia | `33333333-3333-3333-3333-333333333333` |

## Endpoints

Products:

- `GET /products`
- `GET /products/{id}`
- `POST /products`
- `PUT /products/{id}`
- `DELETE /products/{id}`

Orders:

- `GET /orders`
- `GET /orders/{id}`
- `POST /orders`

Product create/update requests require `name`, `description`, `price`, and `stock`. Name and description are limited to 50 characters after trimming. Price must be greater than zero, and stock cannot be negative.

Order creation requires a customer ID and at least one product line. Each line requires a product ID and a positive quantity.

## Pricing Rules

Location pricing is applied first:

- United States: standard pricing
- Europe: 15% increase
- Asia: 5% increase

Then the API selects the single best applicable discount for the customer:

- Volume discount: 10% for 5+ units, 20% for 10+ units, 30% for 50+ units
- Black Friday: 25% discount on the full order
- Holiday: 15% discount on one unit of the most expensive adjusted product

Holiday discounts use Polish bank holidays. Movable holidays are calculated from Gregorian Easter Sunday:

- Easter Sunday
- Easter Monday
- Pentecost
- Corpus Christi

The order date comes from `TimeProvider`, which lets integration tests control seasonal and holiday discount behavior.

## Testing

Run unit tests:

```powershell
dotnet test tests\InventoryManagement.UnitTests\InventoryManagement.UnitTests.csproj
```

Run integration tests:

```powershell
dotnet test tests\InventoryManagement.IntegrationTests\InventoryManagement.IntegrationTests.csproj
```

The unit tests cover core pricing rules. The integration tests cover critical HTTP flows, validation responses, persistence, stock updates, and insufficient-stock behavior.

## Assumptions And Simplifications

- Customer management endpoints are not implemented because the task only requires customers as an order input and location source.
- Customers are seeded instead of managed through the API.
- Location pricing is derived from the seeded customer region.
- Discounts are not combined; the largest discount amount wins.
- Black Friday is treated as the Friday after the fourth Thursday in November.
- Holiday Sales use Polish bank holidays as the reference dates.
- The Holiday discount applies to one unit of the most expensive adjusted product, not the entire order.
- The API includes extra read/update/delete endpoints beyond the minimum task requirements to make product and order workflows easier to verify.

## Trade-Offs

- EF Core migrations are included for schema management, and the API applies migrations at startup for simplicity.
- SQLite keeps the project easy to run locally, but it is not tuned for production-scale concurrency.
- Stock decrements use conditional database updates inside a transaction, so concurrent orders cannot reduce stock below zero.
- Request validation is centralized with FluentValidation, while database-dependent checks such as customer existence, product existence, and stock availability remain in command handlers.
- Minimal APIs keep the project compact; endpoint registration is split by feature to keep `Program.cs` focused on startup.
- Tests are split into unit and integration projects so fast pricing tests do not carry web/database test dependencies.
- Malformed JSON and every possible edge case are not exhaustively tested; coverage focuses on the crucial pricing and HTTP flows from the task.
