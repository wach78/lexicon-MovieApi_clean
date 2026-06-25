using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;

namespace Movie.Presentation.Controllers;

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
    public async Task<ActionResult<PagedResult<MovieAverageRatingDto>>>GetAverageRatingGenre(
            [FromQuery] PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        PagedResult<MovieAverageRatingDto> averageRatings =
            await _serviceManager.Reports.GetAverageRatingsByGenreAsync(
                    paginationParameters,
                    cancellationToken
                );

        return Ok(averageRatings);
    }

    // GET /api/reports/movies/top5pergenre
    [HttpGet("movies/top5pergenre")]
    public async Task<ActionResult<PagedResult<TopMoviesPerGenreDto>>>GetTop5MoviesPerGenre(
            [FromQuery] PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        PagedResult<TopMoviesPerGenreDto> movies =
            await _serviceManager.Reports.GetTopMoviesPerGenreAsync(
                    paginationParameters,
                    cancellationToken
                );

        return Ok(movies);
    }

    // GET /api/reports/actors/most-active
    [HttpGet("actors/most-active")]
    public async Task<ActionResult<PagedResult<MostActiveActorDto>>>GetMostActiveActors(
            [FromQuery] PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        PagedResult<MostActiveActorDto> actors =
            await _serviceManager.Reports.GetMostActiveActorsAsync(
                    paginationParameters,
                    cancellationToken
                );

        return Ok(actors);
    }

    // GET /api/reports/movies/with-most-reviews
    [HttpGet("movies/with-most-reviews")]
    public async Task<ActionResult<MovieWithMostReviewsDto>>GetMovieWithMostReviews(
            CancellationToken cancellationToken = default)
    {
        MovieWithMostReviewsDto? movie =
            await _serviceManager.Reports.GetMovieWithMostReviewsAsync(cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    // GET /api/reports/genres/popular
    [HttpGet("genres/popular")]
    public async Task<ActionResult<PagedResult<PopularGenreDto>>>GetPopularGenre(
            [FromQuery] PaginationParameters paginationParameters,
            CancellationToken cancellationToken = default)
    {
        PagedResult<PopularGenreDto> genres =
            await _serviceManager.Reports.GetPopularGenresAsync(
                    paginationParameters,
                    cancellationToken
                );

        return Ok(genres);
    }
}
