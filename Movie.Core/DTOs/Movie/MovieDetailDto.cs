
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;

namespace Movie.Core.DTOs.Movie;

public class MovieDetailDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public int Year { get; init; }

    public int Duration { get; init; }

    public Guid? GenreId { get; init; }

    public string? GenreName { get; init; }

    public List<ReviewDto> Reviews { get; init; } = new();

    public List<ActorDto> Actors { get; init; } = new();

    public MovieDetailsDto? MovieDetails { get; init; }

}
