using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;

namespace Movie.Services;

public sealed class ReportsService : IReportsService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportsService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<MovieAverageRatingDto>>GetAverageRatingsByGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        return await _unitOfWork.Reports.GetAverageRatingsByGenreAsync(
                paginationParameters,
                cancellationToken
            );
    }

    public async Task<PagedResult<TopMoviesPerGenreDto>>GetTopMoviesPerGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        return await _unitOfWork.Reports.GetTopMoviesPerGenreAsync(
                paginationParameters,
                cancellationToken
            );
    }

    public async Task<PagedResult<MostActiveActorDto>>GetMostActiveActorsAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        return await _unitOfWork.Reports.GetMostActiveActorsAsync(
                paginationParameters,
                cancellationToken
            );
    }

    public async Task<MovieWithMostReviewsDto?>GetMovieWithMostReviewsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports.GetMovieWithMostReviewsAsync(cancellationToken);
    }

    public async Task<PagedResult<PopularGenreDto>>GetPopularGenresAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        return await _unitOfWork.Reports.GetPopularGenresAsync(
                paginationParameters,
                cancellationToken
            );
    }
}
