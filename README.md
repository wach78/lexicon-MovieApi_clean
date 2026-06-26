# Movie API

Movie API is an educational ASP.NET Core Web API project for managing movies, actors, reviews, movie details, and report data.

> This repository is a learning exercise. It is intended to practise API design, layered architecture, Entity Framework Core, testing, pagination, filtering, and API versioning. It is not presented as a production-ready application.

## Table of contents

- [Learning goals](#learning-goals)
- [Features](#features)
- [API versioning](#api-versioning)
- [API documentation](#api-documentation)
- [Endpoints](#endpoints)
- [Pagination and filtering](#pagination-and-filtering)
- [Architecture](#architecture)
- [Domain model](#domain-model)
- [Technologies](#technologies)
- [Prerequisites](#prerequisites)
- [Getting started](#getting-started)
- [Database migrations](#database-migrations)
- [Tests](#tests)
- [Current exercise scope](#current-exercise-scope)

## Learning goals

The project is used to practise:

- REST-style API endpoints with ASP.NET Core controllers
- separation of responsibilities across multiple projects
- DTOs instead of exposing EF Core entities directly
- service and repository abstractions
- the Unit of Work pattern
- dependency injection
- asynchronous database access
- cancellation tokens
- Entity Framework Core relationships
- SQL Server and EF Core migrations
- automatic sample-data seeding
- request validation
- explicit application result handling
- pagination and filtering
- URL-based API versioning
- version-specific OpenAPI documents
- API documentation with Scalar
- unit testing with xUnit and Moq
- code coverage with Coverlet

## Features

### Movies

The API supports:

- listing movies with pagination
- filtering movies by genre, year, and actor
- getting a movie by ID
- getting detailed movie information
- creating movies
- updating movies
- deleting movies
- retrieving related genre, actors, reviews, and movie details

### Actors

The API supports:

- listing actors with pagination
- getting an actor by ID
- creating actors
- updating actors
- adding an existing actor to an existing movie
- preventing the same actor from being added to a movie more than once

### Reviews

The API supports:

- listing reviews with pagination
- listing reviews for a specific movie
- creating a review for a movie
- deleting a review
- partially updating a review in API version 2

The PATCH endpoint uses a partial DTO. It is not an RFC 6902 JSON Patch implementation.

### Reports

The API includes report endpoints for:

- average movie ratings grouped by genre
- the top five movies per genre
- the most active actors
- the movie with the most reviews
- popular genres

### Additional functionality

- URL-based API versioning
- version-specific OpenAPI documents
- Scalar API documentation
- automatic EF Core migration during startup
- automatic sample-data seeding when the movie table is empty
- paged responses
- a default page size of 10
- a maximum page size of 100

## API versioning

The API version is part of the URL:

```text
/api/v1/...
/api/v2/...
```

Version 1 contains the main API.

The movie, actor, and report controllers currently support version 1 only.

The review controller supports both version 1 and version 2. Version 2 also adds the partial review update endpoint:

```http
PATCH /api/v2/movies/{movieId}/reviews/{reviewId}
```

## API documentation

When the application runs in the Development environment, Scalar is available at:

```text
https://localhost:7030/scalar/v1
https://localhost:7030/scalar/v2
```

Version 1 is configured as the default Scalar document.

The corresponding OpenAPI documents are available at:

```text
https://localhost:7030/openapi/v1.json
https://localhost:7030/openapi/v2.json
```

The exact port may differ when a different launch profile or hosting configuration is used.

## Endpoints

### Movies — version 1

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/Movies` | Get paged movies |
| `GET` | `/api/v1/Movies/{id}` | Get a movie by ID |
| `GET` | `/api/v1/Movies/{id}/details` | Get detailed movie data |
| `POST` | `/api/v1/Movies` | Create a movie |
| `PUT` | `/api/v1/Movies/{id}` | Update a movie |
| `DELETE` | `/api/v1/Movies/{id}` | Delete a movie |

### Actors — version 1

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/actors` | Get paged actors |
| `GET` | `/api/v1/actors/{id}` | Get an actor by ID |
| `POST` | `/api/v1/actors` | Create an actor |
| `PUT` | `/api/v1/actors/{id}` | Update an actor |
| `POST` | `/api/v1/movies/{movieId}/actors/{actorId}` | Add an actor to a movie |

### Reviews — version 1

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/reviews` | Get paged reviews |
| `GET` | `/api/v1/movies/{movieId}/reviews` | Get reviews for a movie |
| `POST` | `/api/v1/movies/{movieId}/reviews` | Create a review for a movie |
| `DELETE` | `/api/v1/reviews/{id}` | Delete a review |

### Reviews — version 2

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v2/reviews` | Get paged reviews |
| `GET` | `/api/v2/movies/{movieId}/reviews` | Get reviews for a movie |
| `POST` | `/api/v2/movies/{movieId}/reviews` | Create a review for a movie |
| `DELETE` | `/api/v2/reviews/{id}` | Delete a review |
| `PATCH` | `/api/v2/movies/{movieId}/reviews/{reviewId}` | Partially update a review |

### Reports — version 1

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/reports/movies/average-ratings` | Get average ratings grouped by genre |
| `GET` | `/api/v1/reports/movies/top5pergenre` | Get the top five movies per genre |
| `GET` | `/api/v1/reports/actors/most-active` | Get the most active actors |
| `GET` | `/api/v1/reports/movies/with-most-reviews` | Get the movie with the most reviews |
| `GET` | `/api/v1/reports/genres/popular` | Get popular genres |

## Pagination and filtering

Paged endpoints accept these query parameters:

| Parameter | Description |
|---|---|
| `page` | Page number, starting at 1 |
| `pageSize` | Number of items per page, up to 100 |

The default values are:

```text
page=1
pageSize=10
```

Movie queries also support:

| Parameter | Description |
|---|---|
| `genre` | Filter by genre name |
| `year` | Filter by release year |
| `actor` | Filter by actor name |

Example:

```http
GET /api/v1/Movies?page=1&pageSize=10&genre=Drama&year=1994&actor=Tom%20Hanks
```

A paged response contains:

- `items`
- `totalItems`
- `currentPage`
- `totalPages`
- `pageSize`

## Architecture

The solution is divided into projects with separate responsibilities:

| Project | Responsibility |
|---|---|
| `Movie.Core` | Domain entities, DTOs, constants, pagination models, query parameters, and repository contracts |
| `Movie.Data` | EF Core context, repositories, migrations, seed data, and Unit of Work implementation |
| `Movie.Service.Contracts` | Service interfaces and application result types |
| `Movie.Services` | Business logic and application services |
| `Movie.Presentation` | ASP.NET Core API controllers |
| `MovieApi` | Application host, dependency injection, API versioning, OpenAPI, Scalar, and startup configuration |
| `MovieApi.Tests` | Unit tests for controllers, services, and related application behaviour |

A typical request follows this flow:

```text
HTTP request
    -> Controller
    -> Service
    -> Repository / Unit of Work
    -> Entity Framework Core
    -> SQL Server
```

### Service manager

Controllers depend on `IServiceManager`, which provides access to the application services:

- movie service
- actor service
- review service
- genre service
- report service

### Unit of Work

Services use `IUnitOfWork` to access repositories and commit changes through a shared EF Core context.

## Domain model

The main entities are:

- `Movie`
- `MovieDetails`
- `Actor`
- `Review`
- `Genre`

Important relationships include:

- one movie can have one movie-details record
- one movie can have many reviews
- movies and actors have a many-to-many relationship
- a movie can belong to a genre

The entities use `Guid` identifiers. New domain objects use version 7 GUID generation.

## Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- SQL Server LocalDB
- ASP.NET API Versioning
- Microsoft OpenAPI
- Scalar
- xUnit
- Moq
- Coverlet

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB or another SQL Server instance
- the `dotnet-ef` tool when migrations are managed manually

Install the EF Core CLI tool when required:

```bash
dotnet tool install --global dotnet-ef
```

Update an existing installation with:

```bash
dotnet tool update --global dotnet-ef
```

## Getting started

Clone the repository:

```bash
git clone https://github.com/wach78/lexicon-MovieApi_clean.git
cd lexicon-MovieApi_clean
```

Restore dependencies:

```bash
dotnet restore MovieApi.slnx
```

Build the solution:

```bash
dotnet build MovieApi.slnx
```

Run all tests:

```bash
dotnet test MovieApi.slnx
```

Run the API:

```bash
dotnet run --project MovieApi.csproj
```

The default HTTPS launch profile uses:

```text
https://localhost:7030
```

The default HTTP launch profile uses:

```text
http://localhost:5091
```

### Database connection

The default development connection string uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "MovieApiContext": "Server=(localdb)\\mssqllocaldb;Database=MovieApiContext;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Change the connection string in `appsettings.json` when another SQL Server instance is used.

### Startup behaviour

During startup, the application:

1. creates a dependency-injection scope
2. applies pending EF Core migrations
3. adds sample data when the movie table is empty
4. starts the API

## Database migrations

Apply existing migrations manually:

```bash
dotnet ef database update --project Movie.Data --startup-project .
```

Create a new migration:

```bash
dotnet ef migrations add MigrationName --project Movie.Data --startup-project .
```

Remove the most recent migration before it has been applied:

```bash
dotnet ef migrations remove --project Movie.Data --startup-project .
```

List migrations:

```bash
dotnet ef migrations list --project Movie.Data --startup-project .
```

## Tests

The test project uses:

- xUnit
- Moq
- Microsoft.NET.Test.Sdk
- Coverlet

Run all tests:

```bash
dotnet test MovieApi.slnx
```

Run only the test project:

```bash
dotnet test MovieApi.Tests/MovieApi.Tests.csproj
```

Collect code coverage:

```bash
dotnet test MovieApi.Tests/MovieApi.Tests.csproj --collect:"XPlat Code Coverage"
```

## Current exercise scope

This project is an educational exercise and is still under development.

The following production concerns are currently outside the main scope:

- authentication
- authorization policies
- production deployment configuration
- rate limiting
- a complete global exception-handling strategy
- production logging and monitoring
- API integration tests covering all routes
- a dedicated endpoint for retrieving one review by ID
- creation and update endpoints for movie details
- a complete genre-management controller

The project may continue to change as new course exercises are implemented.
