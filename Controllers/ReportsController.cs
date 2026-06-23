using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Service.Contracts.Interfaces;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReportsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ReportsController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    // GET /api/reports/movies/average-ratings
    [HttpGet("movies/average-ratings")]
    public async Task<
        ActionResult<IReadOnlyList<MovieAverageRatingDto>>>
        GetAverageRatingGenre(
            CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MovieAverageRatingDto> averageRatings =
            await _serviceManager.Reports.GetAverageRatingsByGenreAsync(
                cancellationToken
            );

        return Ok(averageRatings);
    }

    // GET /api/reports/movies/top5pergenre
    [HttpGet("movies/top5pergenre")]
    public async Task<
        ActionResult<IReadOnlyList<TopMoviesPerGenreDto>>>
        GetTop5MoviesPerGenre(
            CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TopMoviesPerGenreDto> movies =
            await _serviceManager.Reports.GetTopMoviesPerGenreAsync(
                cancellationToken
            );

        return Ok(movies);
    }

    // GET /api/reports/actors/most-active
    [HttpGet("actors/most-active")]
    public async Task<
        ActionResult<IReadOnlyList<MostActiveActorDto>>>
        GetMostActiveActors(
            CancellationToken cancellationToken = default)
    {
        IReadOnlyList<MostActiveActorDto> actors =
            await _serviceManager.Reports.GetMostActiveActorsAsync(
                cancellationToken
            );

        return Ok(actors);
    }

    // GET /api/reports/movies/with-most-reviews
    [HttpGet("movies/with-most-reviews")]
    public async Task<ActionResult<MovieWithMostReviewsDto>>
        GetMovieWithMostReviews(
            CancellationToken cancellationToken = default)
    {
        MovieWithMostReviewsDto? movie =
            await _serviceManager.Reports.GetMovieWithMostReviewsAsync(
                cancellationToken
            );

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    // GET /api/reports/genres/popular
    [HttpGet("genres/popular")]
    public async Task<
        ActionResult<IReadOnlyList<PopularGenreDto>>>
        GetPopularGenre(
            CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PopularGenreDto> genres =
            await _serviceManager.Reports.GetPopularGenresAsync(
                cancellationToken
            );

        return Ok(genres);
    }
}
