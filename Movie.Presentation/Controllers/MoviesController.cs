using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.DTOs.Movie;
using Movie.Core.DTOs.Review;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;

namespace Movie.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public MoviesController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    // GET: api/Movie
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
   

    // GET: api/Movie/5
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MovieDto>> GetMovie([FromRoute] Guid id)
    {
        MovieDto? movie = await _serviceManager.Movies.GetMovieByIdAsync(id);

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<MovieDetailDto>> GetMovieDetails([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        MovieDetailDto? movie = await _serviceManager.Movies.GetMovieDetailsAsync(id, cancellationToken);

        if (movie is null)
        {
            return NotFound();
        }

        return Ok(movie);
    }

    // PUT: api/Movie/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutMovie([FromRoute] Guid id, [FromBody] MovieUpdateDto movieUpdateDto, CancellationToken cancellationToken)
    {
        if (id != movieUpdateDto.Id)
        {
            return BadRequest("Route id does not match body id.");
        }

        UpdateMovieResult updateMovieResult = await _serviceManager.Movies.UpdateMovieAsync(id, movieUpdateDto, cancellationToken);

        return updateMovieResult switch
        {
            UpdateMovieResult.MovieNotFound =>
                NotFound("Movie not found."),

            UpdateMovieResult.GenreNotFound =>
                BadRequest("Genre does not exist."),

            UpdateMovieResult.Updated =>
                NoContent(),

            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    // POST: api/Movie
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<MovieDto>> PostMovie([FromBody] MovieCreateDto movieCreateDto, CancellationToken cancellationToken)
    {
        MovieDto? movieDto = await _serviceManager.Movies.CreateMovieAsync(
            movieCreateDto,
            cancellationToken
        );

        if (movieDto is null)
        {
            return BadRequest("Genre does not exist.");
        }

        return CreatedAtAction(
            nameof(GetMovie),
            new { id = movieDto.Id },
            movieDto
        );
    }

    // DELETE: api/Movie/5
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMovie([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        bool isDeleted = await _serviceManager.Movies.DeleteMovieAsync(id, cancellationToken);

        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

