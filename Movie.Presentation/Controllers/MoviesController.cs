using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;

namespace Movie.Presentation.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IServiceManager serviceManager, ILogger<MoviesController> logger)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of movies.
    /// </summary>
    /// <param name="queryParameters">
    /// Pagination and filtering parameters used to retrieve movies.
    /// Movies can be filtered by genre, release year, and actor name.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of movies.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated list of movies.
    /// </response>
    /// <response code="400">
    /// The supplied pagination or filtering parameters are invalid.
    /// </response>

    [HttpGet]
    public async Task<ActionResult<PagedResult<MovieDto>>> GetMovies([FromQuery] MovieQueryParameters queryParameters,CancellationToken cancellationToken)
    {
        PagedResult<MovieDto> result =
            await _serviceManager.Movies.GetMoviesAsync(
                queryParameters,
                cancellationToken
            );

        return Ok(result);
    }

    /// <summary>
    /// Gets a movie by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The requested movie.
    /// </returns>
    /// <response code="200">
    /// Returns the requested movie.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    /// </response>

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MovieDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDto>> GetMovie([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        MovieDto? movie = await _serviceManager.Movies.GetMovieByIdAsync(id, cancellationToken);

        if (movie is null)
        {
            _logger.LogInformation("Movie with ID {MovieId} was not found",
                id);
            return NotFound();
        }

        _logger.LogDebug("Movie with ID {MovieId} was retrieved successfully",
            id);
        return Ok(movie);
    }

    /// <summary>
    /// Gets detailed information about a movie by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// Detailed information about the requested movie.
    /// </returns>
    /// <response code="200">
    /// Returns the detailed information about the movie.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    /// </response>
    
    [HttpGet("{id:guid}/details")]
    [ProducesResponseType(typeof(MovieDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailDto>> GetMovieDetails([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        MovieDetailDto? movie = await _serviceManager.Movies.GetMovieDetailsAsync(id, cancellationToken);

        if (movie is null)
        {
            _logger.LogInformation("Movie with ID {MovieId} was not found",
                id);
            return NotFound();
        }

        _logger.LogDebug("Movie details with ID {MovieId} was retrieved successfully",
            id);
        return Ok(movie);
    }

    /// <summary>
    /// Updates an existing movie.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the movie to update.
    /// </param>
    /// <param name="movieUpdateDto">
    /// The updated movie information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the movie was updated successfully.
    /// </returns>
    /// <response code="204">
    /// The movie was updated successfully.
    /// </response>
    /// <response code="400">
    /// The route identifier does not match the identifier in the request body,
    /// the supplied data is invalid, or the specified genre does not exist.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    /// </response>
    /// <response code="500">
    /// An unexpected server error occurred.
    /// </response>
    
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutMovie([FromRoute] Guid id, [FromBody] MovieUpdateDto movieUpdateDto, CancellationToken cancellationToken)
    {
        if (id != movieUpdateDto.Id)
        {
            _logger.LogWarning("Movie update rejected because route ID {RouteMovieId} does not match body ID {BodyMovieId}",
                id,
                movieUpdateDto.Id);

            return BadRequest("Route id does not match body id.");
        }

        UpdateMovieResult updateMovieResult = await _serviceManager.Movies.UpdateMovieAsync(id, movieUpdateDto, cancellationToken);

        switch (updateMovieResult)
        {
            case UpdateMovieResult.MovieNotFound:
                _logger.LogInformation("Movie with ID {MovieId} could not be updated because it was not found",
                    id);

                return NotFound("Movie not found.");

            case UpdateMovieResult.GenreNotFound:
                _logger.LogInformation("Movie {MovieId} could not be updated because genre {GenreId} does not exist",
                    id,
                    movieUpdateDto.GenreId);

                return BadRequest("Genre does not exist.");

            case UpdateMovieResult.Updated:
                _logger.LogInformation("Movie with ID {MovieId} was updated successfully",
                    id);

                return NoContent();

            default:
                _logger.LogError("Movie update returned an unexpected result {UpdateResult} for movie {MovieId}",
                    updateMovieResult,
                    id);

                return StatusCode(
                    StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="movieCreateDto">
    /// The information required to create the movie.
    /// </param> /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The newly created movie.
    /// </returns>
    /// <response code="201">
    /// The movie was created successfully.
    /// </response>
    /// <response code="400">
    /// The supplied movie data is invalid or the specified genre does not exist.
    /// </response>
    /// 
    [HttpPost]
    [ProducesResponseType(typeof(MovieDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDto>> PostMovie([FromBody] MovieCreateDto movieCreateDto, CancellationToken cancellationToken)
    {
        MovieDto? movieDto = await _serviceManager.Movies.CreateMovieAsync(
            movieCreateDto,
            cancellationToken
        );

        if (movieDto is null)
        {
            _logger.LogWarning("Movie creation failed because genre {GenreId} does not exist",
                movieCreateDto.GenreId);

            return BadRequest("Genre does not exist.");
        }

        _logger.LogInformation("Movie with ID {MovieId} was created successfully",
            movieDto.Id);

        return CreatedAtAction(
            nameof(GetMovie),
            new { id = movieDto.Id },
            movieDto
        );
    }

    /// <summary>
    /// Deletes a movie by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the movie to delete.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the movie was deleted.
    /// </returns>
    /// <response code="204">
    /// The movie was deleted successfully.
    /// </response>
    /// <response code="404">
    /// The movie was not found.
    /// </response>

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
   
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        bool isDeleted = await _serviceManager.Movies.DeleteMovieAsync(id, cancellationToken);

        if (!isDeleted)
        {
            _logger.LogInformation("Movie with ID {MovieId} was not found",
                id);
            return NotFound();
        }

        _logger.LogInformation("Movie with ID {MovieId} was deleted",
            id);
        return NoContent();
    }
}

