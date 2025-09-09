# Total Time to finish this task is 20hrs
# Price/ Pricing API (.NET 8 Minimal APIs)

Manage suppliers, products, and price lists, and query the best price for a given SKU. Built with Minimal APIs, EF Core, and a Clean Architecture layout.

## Features

- Suppliers & Products: CRUD
- Price Lists: CSV upload with overlap validation
- Best Price: currency conversion, tie-breakers, and caching
- Pagination & filtering for prices
- ProblemDetails error responses, logging, async/await, DI
- SQLite with migrations and seed data (suppliers/products)
- Swagger/OpenAPI (file upload supported)
- Refit client interfaces (optional) for typed API consumption

## Project Layout

/src /Pricing.Api # Minimal API endpoints only (or Price.Api if you named it so) /Pricing.Application # Use cases, DTOs, interfaces /Pricing.Domain # Entities, value objects /Pricing.Infrastructure # EF Core, repositories, rate provider, caching, mappers /tests /Pricing.Tests # Unit & integration tests


## Prerequisites

- .NET 8 SDK
- SQLite (no server needed)
- Optional: dotnet-ef (for migrations)
  - Install: `dotnet tool install --global dotnet-ef`

## Configuration

- SQLite database stored under `App_Data/pricing.db` by default (created on first run).
- Configure via `appsettings.json` or environment variables:
```json
{
  "ConnectionStrings": {
    "Pricing": "Data Source=App_Data/pricing.db;Cache=Shared"
  }
}
Restore, Build, Run
dotnet restore
dotnet build
# If your API project is named Pricing.Api:
dotnet run --project src/Pricing.Api
# If you named it Price.Api:
# dotnet run --project src/Price.Api
Swagger UI: open the URL printed by dotnet run (e.g., http://localhost:5xxx/swagger)
You should see a file picker for /prices/upload
Database Initialization
On startup the app will:

Apply migrations if any exist, otherwise call EnsureCreated()
Seed suppliers and products if tables are empty
To create a migration and update the DB manually:

# Adjust paths if your project names differ (Price.Api vs Pricing.Api)
dotnet ef migrations add InitialCreate -p src/Pricing.Infrastructure -s src/Pricing.Api
dotnet ef database update -p src/Pricing.Infrastructure -s src/Pricing.Api
If you changed DB location and need a clean slate:

rm -f App_Data/pricing.db App_Data/pricing.db-wal App_Data/pricing.db-shm
dotnet ef database update -p src/Pricing.Infrastructure -s src/Pricing.Api

CSV Upload (Price Lists)
Endpoint: POST /prices/upload (multipart/form-data with field name file)

CSV columns: SupplierId,Sku,ValidFrom,ValidTo,Currency,PricePerUom,MinQty

Overlap validation: rejects overlapping date ranges for the same (SupplierId, Sku)

Sample CSV (save as PriceLists.csv)

SupplierId,Sku,ValidFrom,ValidTo,Currency,PricePerUom,MinQty
1,ABC123,2025-08-01,2025-12-31,EUR,9.50,100
2,ABC123,2025-07-01,2025-10-31,USD,10.00,50
3,ABC123,2025-08-15,2025-12-31,EUR,9.50,100
1,XYZ777,2025-08-01,2025-12-31,EUR,5.25,10
2,XYZ777,2025-09-01,2025-11-30,USD,5.40,10

Upload (Windows PowerShell)
Invoke-WebRequest -Uri "http://localhost:5xxx/prices/upload" -Method Post -Form @{ file = Get-Item .\PriceLists.csv }

Upload (bash / WSL / macOS)
curl -X POST "http://localhost:5xxx/prices/upload" -F "file=@PriceLists.csv;type=text/csv"

Note: Do not set a manual Content-Type header when using -F.

#Endpoints

Suppliers:

GET /suppliers
GET /suppliers/{id}
POST /suppliers
PUT /suppliers/{id}
DELETE /suppliers/{id}

Products:

GET /products
GET /products/{id}
POST /products
PUT /products/{id}
DELETE /products/{id}

Price Lists:

POST /prices/upload (multipart/form-data: file)
GET /prices?sku=&validOn=&currency=&supplierId=&page=&pageSize=

Best Price:

GET /pricing/best?sku=...&qty=...&currency=...&date=YYYY-MM-DD

#Examples
List prices:
curl "http://localhost:5xxx/prices?page=1&pageSize=50"
curl "http://localhost:5xxx/prices?sku=ABC123&validOn=2025-09-01&page=1&pageSize=50"
Best price:
curl "http://localhost:5xxx/pricing/best?sku=ABC123&qty=120&currency=EUR&date=2025-09-01"

#Architecture Notes
Clean separation:
Domain: entities only
Application: DTOs, interfaces, use cases
Infrastructure: EF Core, repositories, caching, static rate provider, mappers
Api: OpenAPI, endpoints, no business logic
EF Core best practices:
AsNoTracking for reads
Server-side projections .Select(new Dto(...))
Best-price tie-breakers:
Lowest converted unit price
Preferred supplier
Shortest lead time
Lowest supplier ID
Currency conversion: static in-memory provider
Caching: in-memory short TTL for frequent best-price queries

## Healthchecks
- Liveness: `/health/live`
- Readiness: `/health/ready`

Test log: http://localhost:8080/testLog
Live: http://localhost:8080/health/live
Ready: http://localhost:8080/health/ready

#Refit Clients (Optional)
Use these if you want typed HTTP clients from other apps/tests:

// Example DI registration (consumer app)
services.AddRefitClient<Pricing.Client.Apis.IPricesApi>()
	.ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5xxx"));

#Testing
dotnet test
Includes unit tests for business rules and at least one integration test for best-price.

#Troubleshooting
SQLite �no such table�: ensure migrations ran or EnsureCreated() executed; delete old DB files in App_Data if schema changed.
Swagger doesn�t show file input: The upload endpoint must accept [FromForm] IFormFile file and be marked as multipart/form-data in OpenAPI.
500 on CSV upload: Don�t override Content-Type when using curl -F. Ensure dates are yyyy-MM-dd.
Extra SQLite files (.db-wal, .db-shm): They�re normal when WAL is enabled. To avoid them, add ;Journal Mode=Delete to the connection string (slower).

#Deployment
Repository have Dockerfile, Docker-Compose & K8s files [Namespace, Deployment, NodePort Service].yaml
