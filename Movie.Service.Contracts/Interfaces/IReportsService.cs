using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;

namespace Movie.Service.Contracts.Interfaces;

public interface IReportsService
{
    Task<IReadOnlyList<MovieAverageRatingDto>>
        GetAverageRatingsByGenreAsync(
            CancellationToken cancellationToken = default
        );

    Task<IReadOnlyList<TopMoviesPerGenreDto>>
        GetTopMoviesPerGenreAsync(
            CancellationToken cancellationToken = default
        );

    Task<IReadOnlyList<MostActiveActorDto>>
        GetMostActiveActorsAsync(
            CancellationToken cancellationToken = default
        );

    Task<MovieWithMostReviewsDto?> GetMovieWithMostReviewsAsync(
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<PopularGenreDto>> GetPopularGenresAsync(
        CancellationToken cancellationToken = default
    );
}
