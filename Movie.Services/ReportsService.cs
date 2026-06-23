using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
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

    public async Task<IReadOnlyList<MovieAverageRatingDto>>
        GetAverageRatingsByGenreAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports
            .GetAverageRatingsByGenreAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TopMoviesPerGenreDto>>
        GetTopMoviesPerGenreAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports
            .GetTopMoviesPerGenreAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MostActiveActorDto>>
        GetMostActiveActorsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports
            .GetMostActiveActorsAsync(cancellationToken);
    }

    public async Task<MovieWithMostReviewsDto?>
        GetMovieWithMostReviewsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports
            .GetMovieWithMostReviewsAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PopularGenreDto>>
        GetPopularGenresAsync(
            CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Reports
            .GetPopularGenresAsync(cancellationToken);
    }
}
