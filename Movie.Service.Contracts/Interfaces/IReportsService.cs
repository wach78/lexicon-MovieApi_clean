using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;

namespace Movie.Service.Contracts.Interfaces;

public interface IReportsService
{
    Task<PagedResult<MovieAverageRatingDto>>
        GetAverageRatingsByGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default
        );

    Task<PagedResult<TopMoviesPerGenreDto>>
        GetTopMoviesPerGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default
        );

    Task<PagedResult<MostActiveActorDto>>
        GetMostActiveActorsAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default
        );

    Task<MovieWithMostReviewsDto?> GetMovieWithMostReviewsAsync(
        CancellationToken cancellationToken = default
    );

    Task<PagedResult<PopularGenreDto>> GetPopularGenresAsync(
        PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default
    );
}
