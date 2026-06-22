namespace Movie.Core.DTOs.Movie;

public sealed class MovieWithMostReviewsDto
{
    public Guid MovieId { get; init; }

    public string Title { get; init; } = string.Empty;

    public int ReviewCount { get; init; }
}
