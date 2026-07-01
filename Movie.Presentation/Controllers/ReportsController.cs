using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Report;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;

namespace Movie.Presentation.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ReportsController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    /// <summary>
    /// Gets the average movie rating for each genre.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve the report.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection containing the average movie rating
    /// for each genre.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated average-rating report.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>

    [HttpGet("movies/average-ratings")]
    [ProducesResponseType(typeof(PagedResult<MovieAverageRatingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<MovieAverageRatingDto>>> GetAverageRatingGenre(
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

    /// <summary>
    /// Gets the five highest-rated movies for each genre.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve the report.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection containing the top five movies
    /// for each genre.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated top-movies-per-genre report.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>

    [HttpGet("movies/top5pergenre")]
    [ProducesResponseType(typeof(PagedResult<TopMoviesPerGenreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<TopMoviesPerGenreDto>>> GetTop5MoviesPerGenre(
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

    /// <summary>
    /// Gets the actors who have appeared in the most movies.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve the report.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of the most active actors.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated most-active-actors report.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>

    [HttpGet("actors/most-active")]
    [ProducesResponseType(typeof(PagedResult<MostActiveActorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<MostActiveActorDto>>> GetMostActiveActors(
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

    /// <summary>
    /// Gets the movie that has received the most reviews.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The movie with the highest number of reviews.
    /// </returns>
    /// <response code="200">
    /// Returns the movie with the most reviews.
    /// </response>
    /// <response code="404">
    /// No reviewed movie was found.
    /// </response>

    [HttpGet("movies/with-most-reviews")]
    [ProducesResponseType(typeof(MovieWithMostReviewsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieWithMostReviewsDto>> GetMovieWithMostReviews(
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

    /// <summary>
    /// Gets genres ordered by their popularity.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve the report.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of popular genres.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated popular-genres report.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid.
    /// </response>

    [HttpGet("genres/popular")]
    [ProducesResponseType(typeof(PagedResult<PopularGenreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PopularGenreDto>>> GetPopularGenre(
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
