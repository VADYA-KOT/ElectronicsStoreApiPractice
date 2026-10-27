# ElectronicsStoreApiPractice

A REST API for managing an electronics store, built with C# and ASP.NET Core. It provides product and category management, JWT-based authentication, role-based authorization, PostgreSQL persistence, request validation, and automated tests.

## Technologies

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- JWT Bearer Authentication
- Docker and Docker Compose
- xUnit
- ASP.NET Core integration testing

## Features

### Products
- Create, retrieve, update, and delete products.
- Retrieve products by ID or category.
- Search products by name.
- Filter by category, price range, maximum quantity, and stock availability.
- Sort by supported fields in ascending or descending order.
- Paginate results.
- Validate incoming data and category references.

### Categories
- Retrieve all categories or a category by ID.
- Create categories with input validation.
- Return category information, including product counts.

### Authentication and authorization
- Register users and authenticate with a username and password.
- Hash passwords using ASP.NET Core Identity password-hashing services.
- Issue JWT access tokens.
- Validate token signatures, issuer, audience, and expiration.
- Restrict protected operations by user role.

### Database and validation
- Use Entity Framework Core to access PostgreSQL.
- Manage database schema changes through migrations.
- Enforce database constraints for positive product prices and non-negative quantities.
- Use request DTOs and response DTOs to separate API contracts from database entities.

### Testing
- Unit tests for product service behaviour and request validation.
- Integration tests for HTTP endpoints, authentication, authorization, filtering, sorting, and pagination.

The automated tests use an isolated EF Core InMemory database where configured. This does not replace testing against a real PostgreSQL instance.

## Getting started

### Option 1: Run with Docker Compose

Prerequisites:
- Docker Desktop with Docker Compose.
- The .NET 10 SDK and the EF Core CLI if you need to apply migrations from the host machine.

Create a file named `.env` in the repository root. Do not commit this file:

```dotenv
POSTGRES_DB=electronics_store
POSTGRES_USER=store_user
POSTGRES_PASSWORD=replace_with_a_local_password
JWT_ISSUER=ElectronicsStoreApiPractice
JWT_AUDIENCE=ElectronicsStoreApiPracticeClient
JWT_KEY=replace_with_a_base64_encoded_key
```

The JWT key must be valid Base64 and decode to a sufficiently long random key. You can generate a 32-byte key in PowerShell:

```powershell
[Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
```

Copy the generated value into `JWT_KEY` in `.env`. Use your own local database password. Never publish real credentials or signing keys.

Build and start the containers from the repository root:

```powershell
docker compose up -d --build
docker compose ps
docker compose logs api
```

The API is exposed at `http://localhost:8081`. PostgreSQL is exposed to the host at port `5435`; inside the Compose network, the API connects to the database service using host `db` and port `5432`.

#### Database migrations

Docker Compose runs a dedicated `migrate` service after PostgreSQL becomes healthy. The service applies Entity Framework Core migrations and exits; the API starts only if the migration service completes successfully. You do not need to run `dotnet ef database update` manually when starting the stack with Compose.

The migration service uses the .NET SDK image and the EF Core CLI version matching the project's EF Core tools package. It receives the database connection string and JWT configuration through environment variables, just like the API. Keep the `.env` file local and never commit real credentials or signing keys.

To inspect the migration service:

```powershell
docker compose logs migrate
docker compose ps -a
```

If a migration fails, the API will not start. Review the migration logs, correct the reported configuration or migration issue, and rerun:

```powershell
docker compose up -d --build
```

Useful commands:

```powershell
docker compose ps
docker compose logs api
docker compose logs db
docker compose down
```

The named database volume preserves PostgreSQL data when containers are stopped or removed with `docker compose down`. **Do not run `docker compose down -v` unless you intentionally want to delete the database volume and its data.**

### Option 2: Run locally

Prerequisites:
- .NET 10 SDK
- PostgreSQL
- Visual Studio or another .NET-compatible development environment

#### Configuration

Configure the following settings using .NET User Secrets or environment variables:

- `ConnectionStrings:DefaultConnection` — PostgreSQL connection string.
- `Jwt:Issuer` — expected token issuer.
- `Jwt:Audience` — expected token audience.
- `Jwt:Key` — Base64-encoded symmetric signing key.

Do not commit database passwords, signing keys, or other secrets to source control.

#### Database setup

1. Create a PostgreSQL database.
2. Configure the connection string and JWT settings.
3. Apply the existing EF Core migrations using the Package Manager Console with the API project selected:

   `Update-Database`

#### Run the application

Open the solution in Visual Studio, select the API project as the startup project, and run it.

The local HTTP and HTTPS addresses depend on the launch profile configured for the application.

The OpenAPI document is available in the Development environment at the application's `/openapi/v1.json` endpoint.

## API endpoints

| Method | Endpoint | Purpose |
|---|---|---|
| POST | `/api/auth/register` | Register an account |
| POST | `/api/auth/login` | Authenticate and obtain a token |
| GET | `/api/products` | List, search, filter, sort, and paginate products |
| GET | `/api/products/{id}` | Retrieve a product |
| GET | `/api/products/by-category/{categoryId}` | Retrieve products by category |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |
| GET | `/api/categories` | List categories |
| GET | `/api/categories/{id}` | Retrieve a category |
| POST | `/api/categories` | Create a category |

Protected product-management and category-creation endpoints require the appropriate authorization role.

## Project status

The API combines product and category management, authentication and authorization, database persistence, validation, containerization, and automated testing in a single backend service.
