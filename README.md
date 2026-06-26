# Movie API

Movie API is an educational ASP.NET Core Web API project for managing movies, actors, reviews, movie details, and reporting data.

> This repository is a learning exercise. It is intended to practise API design, layered architecture, Entity Framework Core, testing, pagination, and API versioning. It is not presented as a production-ready application.

## Learning goals

The project is used to practise:

- REST-style API endpoints with ASP.NET Core controllers
- separation of responsibilities across multiple projects
- DTOs instead of exposing EF Core entities directly
- service and repository abstractions
- the Unit of Work pattern
- asynchronous database access with cancellation tokens
- Entity Framework Core relationships, migrations, and seeding
- validation and explicit result handling
- pagination and filtering
- API versioning through URL segments
- OpenAPI documentation with Scalar
- unit testing with xUnit and Moq

## Features

### Movies

- list movies with pagination
- filter movies by genre, year, and actor
- get a movie by ID
- get a detailed movie response
- create, update, and delete movies
- connect movies to genres, actors, reviews, and movie details

### Actors

- list actors with pagination
- get an actor by ID
- create and update actors
- add an existing actor to an existing movie
- prevent the same actor from being added to a movie more than once

### Reviews

- list reviews with pagination
- list reviews for a specific movie
- create a review for a movie
- delete a review
- partially update a review through the version 2 PATCH endpoint

### Reports

The API includes report endpoints for:

- average movie ratings grouped by genre
- top movies per genre
- most active actors
- the movie with the most reviews
- popular genres

### Additional functionality

- URL-based API versioning, for example `/api/v1/...` and `/api/v2/...`
- version-specific OpenAPI documents
- Scalar API documentation with version 1 as the default document
- automatic EF Core migration and sample-data seeding when the application starts
- paged responses with a default page size of 10 and a maximum page size of 100

## API versions

The API version is part of the URL:

```text
/api/v1/...
/api/v2/...
```

Version 1 contains the main API. Version 2 currently adds the partial review update endpoint:

```http
PATCH /api/v2/movies/{movieId}/reviews/{reviewId}
```

Scalar documentation is available at:

```text
https://localhost:7030/scalar/v1
https://localhost:7030/scalar/v2
```

The corresponding OpenAPI documents are available at:

```text
https://localhost:7030/openapi/v1.json
https://localhost:7030/openapi/v2.json
```

## Example routes

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/v1/Movies` | Get paged movies |
| `GET` | `/api/v1/Movies/{id}` | Get a movie |
| `GET` | `/api/v1/Movies/{id}/details` | Get detailed movie data |
| `POST` | `/api/v1/Movies` | Create a movie |
| `PUT` | `/api/v1/Movies/{id}` | Update a movie |
| `DELETE` | `/api/v1/Movies/{id}` | Delete a movie |
| `GET` | `/api/v1/actors` | Get paged actors |
| `POST` | `/api/v1/movies/{movieId}/actors/{actorId}` | Add an actor to a movie |
| `GET` | `/api/v1/reviews` | Get paged reviews |
| `GET` | `/api/v1/movies/{movieId}/reviews` | Get reviews for a movie |
| `POST` | `/api/v1/movies/{movieId}/reviews` | Create a review |
| `PATCH` | `/api/v2/movies/{movieId}/reviews/{reviewId}` | Partially update a review |
| `GET` | `/api/v1/reports/movies/average-ratings` | Get average ratings by genre |
| `GET` | `/api/v1/reports/movies/top5pergenre` | Get top movies per genre |
| `GET` | `/api/v1/reports/actors/most-active` | Get the most active actors |
| `GET` | `/api/v1/reports/movies/with-most-reviews` | Get the movie with most reviews |
| `GET` | `/api/v1/reports/genres/popular` | Get popular genres |

Example pagination and filtering request:

```http
GET /api/v1/Movies?page=1&pageSize=10&genre=Drama&year=1994&actor=Tom%20Hanks
```

## Architecture

The solution is divided into projects with separate responsibilities:

| Project | Responsibility |
|---|---|
| `Movie.Core` | Domain entities, DTOs, constants, pagination models, query parameters, and repository contracts |
| `Movie.Data` | EF Core context, repositories, migrations, seed data, and Unit of Work implementation |
| `Movie.Service.Contracts` | Service interfaces and application result types |
| `Movie.Services` | Business logic and application services |
| `Movie.Presentation` | ASP.NET Core API controllers |
| `MovieApi` | Application host, dependency injection, API versioning, OpenAPI, and startup configuration |
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
- movies can belong to a genre

The entities use `Guid` identifiers, including version 7 GUID generation for new domain objects.

## Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server / SQL Server LocalDB
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

## Getting started

Clone the repository:

```bash
git clone https://github.com/wach78/lexicon-MovieApi_clean.git
cd lexicon-MovieApi_clean
```

Restore and build the solution:

```bash
dotnet restore MovieApi.slnx
dotnet build MovieApi.slnx
```

The default development connection string uses SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "MovieApiContext": "Server=(localdb)\\mssqllocaldb;Database=MovieApiContext;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Change the connection string in `appsettings.json` when using another SQL Server instance.

Run the API:

```bash
dotnet run --project MovieApi.csproj
```

During startup, the application applies pending EF Core migrations and adds sample data when the movie table is empty.

## Database migrations

The application normally applies migrations automatically during startup. They can also be applied manually from the repository root:

```bash
dotnet ef database update --project Movie.Data --startup-project .
```

Create a new migration with:

```bash
dotnet ef migrations add MigrationName --project Movie.Data --startup-project .
```

## Tests

Run all tests:

```bash
dotnet test MovieApi.slnx
```

Collect code coverage:

```bash
dotnet test MovieApi.Tests/MovieApi.Tests.csproj --collect:"XPlat Code Coverage"
```

The test project uses xUnit, Moq, and Coverlet.

## Exercise scope

Because this is a learning project, some production concerns are intentionally outside the current scope, including authentication, authorization policies, deployment configuration, and a complete global error-handling strategy. The project may continue to change as new course exercises are implemented.
