using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Microsoft.Extensions.Logging;

namespace Movie.Services;

public sealed class ReportsService : IReportsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportsService> _logger;

    public ReportsService(IUnitOfWork unitOfWork, ILogger<ReportsService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<MovieAverageRatingDto>>GetAverageRatingsByGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving average movie ratings by genre for page {PageNumber} with page size {PageSize}",
            paginationParameters.Page,
            paginationParameters.PageSize);

        PagedResult<MovieAverageRatingDto> result =
            await _unitOfWork.Reports.GetAverageRatingsByGenreAsync(
                paginationParameters,
                cancellationToken);

        _logger.LogDebug("Retrieved {ReturnedItemCount} average-rating records from a total of {TotalItemCount} on page {CurrentPage}",
            result.Items.Count,
            result.TotalItems,
            result.CurrentPage);

        return result;
    }

    public async Task<PagedResult<TopMoviesPerGenreDto>>GetTopMoviesPerGenreAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving top movies per genre for page {PageNumber} with page size {PageSize}",
            paginationParameters.Page,
            paginationParameters.PageSize);

        PagedResult<TopMoviesPerGenreDto> result =
            await _unitOfWork.Reports.GetTopMoviesPerGenreAsync(
                paginationParameters,
                cancellationToken);

        _logger.LogDebug(
            "Retrieved {ReturnedItemCount} top-movie records from a total of {TotalItemCount} on page {CurrentPage}",
            result.Items.Count,
            result.TotalItems,
            result.CurrentPage);

        return result;
    }

    public async Task<PagedResult<MostActiveActorDto>>GetMostActiveActorsAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving most active actors for page {PageNumber} with page size {PageSize}",
            paginationParameters.Page,
            paginationParameters.PageSize);

        PagedResult<MostActiveActorDto> result =
            await _unitOfWork.Reports.GetMostActiveActorsAsync(
                paginationParameters,
                cancellationToken);

        _logger.LogDebug("Retrieved {ReturnedActorCount} most-active-actor records from a total of {TotalActorCount} on page {CurrentPage}",
            result.Items.Count,
            result.TotalItems,
            result.CurrentPage);

        return result;
    }

    public async Task<MovieWithMostReviewsDto?>GetMovieWithMostReviewsAsync(
            CancellationToken cancellationToken = default)
    {
        MovieWithMostReviewsDto? movie = await _unitOfWork.Reports.GetMovieWithMostReviewsAsync(
           cancellationToken);

        if (movie is null)
        {
            _logger.LogInformation("No movie with reviews was found");

            return null;
        }

        _logger.LogDebug("Retrieved the movie with the most reviews");

        return movie;
    }

    public async Task<PagedResult<PopularGenreDto>>GetPopularGenresAsync(
            PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(paginationParameters);

        _logger.LogDebug("Retrieving popular genres for page {PageNumber} with page size {PageSize}",
               paginationParameters.Page,
               paginationParameters.PageSize);

        PagedResult<PopularGenreDto> result =
            await _unitOfWork.Reports.GetPopularGenresAsync(
                paginationParameters,
                cancellationToken);

        _logger.LogDebug("Retrieved {ReturnedGenreCount} popular genres from a total of {TotalGenreCount} on page {CurrentPage}",
            result.Items.Count,
            result.TotalItems,
            result.CurrentPage);

        return result;
    }
}
