# ElectronicsStoreApiPractice

An educational REST API for managing an electronics store, built with C# and ASP.NET Core.

The project demonstrates backend development concepts, database integration, authentication, authorization, input validation, and automated testing.

## Technologies

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql
- JWT Bearer Authentication
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

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Visual Studio or another .NET-compatible development environment

### Configuration

Configure the following settings using .NET User Secrets or environment variables:

- `ConnectionStrings:DefaultConnection` — PostgreSQL connection string.
- `Jwt:Issuer` — expected token issuer.
- `Jwt:Audience` — expected token audience.
- `Jwt:Key` — Base64-encoded symmetric signing key.

Do not commit database passwords, signing keys, or other secrets to source control.

### Database setup

1. Create a PostgreSQL database.
2. Configure the connection string and JWT settings.
3. Apply the existing EF Core migrations using the Package Manager Console with the API project selected:

   `Update-Database`

### Run the application

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

This is a learning and portfolio project developed to practise building and testing an ASP.NET Core backend API.

The project is intended to demonstrate an understanding of API design, data persistence, authentication, authorization, validation, and automated testing.