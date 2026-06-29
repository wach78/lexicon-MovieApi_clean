using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;

namespace Movie.Presentation.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}")]
public sealed class ActorsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ActorsController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    /// <summary>
    /// Adds an existing actor to an existing movie.
    /// </summary>
    /// <param name="movieId">
    /// The unique identifier of the movie.
    /// </param>
    /// <param name="actorId">
    /// The unique identifier of the actor to add.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the actor was added to the movie.
    /// </returns>
    /// <response code="204">
    /// The actor was added to the movie successfully.
    /// </response>
    /// <response code="400">
    /// The actor is already associated with the movie.
    /// </response>
    /// <response code="404">
    /// The movie or actor was not found.
    /// </response>
    /// <response code="500">
    /// An unexpected server error occurred.
    /// </response>
    /// 
    [HttpPost("movies/{movieId:guid}/actors/{actorId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddActorToMovie(
        [FromRoute] Guid movieId,
        [FromRoute] Guid actorId,
        CancellationToken cancellationToken = default)
    {
        AddActorToMovieResult result =
            await _serviceManager.Actors.AddActorToMovieAsync(
                movieId,
                actorId,
                cancellationToken
            );

        return result switch
        {
            AddActorToMovieResult.Added => NoContent(),

            AddActorToMovieResult.MovieNotFound =>
                NotFound("Movie was not found."),

            AddActorToMovieResult.ActorNotFound =>
                NotFound("Actor was not found."),

            AddActorToMovieResult.ActorAlreadyAdded =>
                BadRequest("Actor is already added to this movie."),

            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    /// <summary>
    /// Gets a paginated list of actors.
    /// </summary>
    /// <param name="paginationParameters">
    /// The pagination parameters used to retrieve actors.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A paginated collection of actors.
    /// </returns>
    /// <response code="200">
    /// Returns the paginated list of actors.
    /// </response>
    /// <response code="400">
    /// The supplied pagination parameters are invalid. /
    /// </response>
    /// 
    [HttpGet("actors")]
    [ProducesResponseType(typeof(ActorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ActorDto>>> GetActors(
        [FromQuery] PaginationParameters paginationParameters,
        CancellationToken cancellationToken = default)
    {
        PagedResult<ActorDto> result =
            await _serviceManager.Actors.GetActorsAsync(
                paginationParameters,
                cancellationToken
            );

        return Ok(result);
    }

    /// <summary>
    /// Gets an actor by its unique identifier.
    /// </summary>
    /// <param name="id">
    ///  The unique identifier of the actor.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The requested actor.
    /// </returns>
    /// <response code="200">
    /// Returns the requested actor.
    /// </response>
    /// <response code="404">
    /// The actor was not found.
    /// </response>
    [HttpGet("actors/{id:guid}")]
    [ProducesResponseType(typeof(ActorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActorDto>> GetActor(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        ActorDto? actor = await _serviceManager.Actors.GetActorByIdAsync(
            id,
            cancellationToken
        );

        if (actor is null)
        {
            return NotFound();
        }

        return Ok(actor);
    }
    /// <summary>
    /// Creates a new actor.
    /// </summary>
    /// <param name="actorCreateDto">
    /// The information required to create the actor.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// The newly created actor.
    /// </returns>
    /// <response code="201">
    /// The actor was created successfully.
    /// </response>
    /// <response code="400">
    /// The supplied actor data is invalid.
    /// </response>

    [HttpPost("actors")]
    [ProducesResponseType(typeof(ActorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ActorDto>> PostActor(
        [FromBody] ActorCreateDto actorCreateDto,
        CancellationToken cancellationToken = default)
    {
        ActorDto actor = await _serviceManager.Actors.CreateActorAsync(
            actorCreateDto,
            cancellationToken
        );

        return CreatedAtAction(
            nameof(GetActor),
            new { id = actor.Id },
            actor
        );
    }

    /// <summary>
    /// Updates an existing actor.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the actor to update.
    /// </param>
    /// <param name="actorUpdateDto">
    /// The updated actor information.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A result indicating whether the actor was updated successfully.
    /// </returns>
    /// <response code="204">
    /// The actor was updated successfully.
    /// </response>
    /// <response code="400">
    /// The route identifier does not match the identifier in the request body,
    /// or the supplied actor data is invalid.
    /// </response>
    /// <response code="404">
    /// The actor was not found.
    /// </response>

    [HttpPut("actors/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutActor(
        [FromRoute] Guid id,
        [FromBody] ActorUpdateDto actorUpdateDto,
        CancellationToken cancellationToken = default)
    {
        if (id != actorUpdateDto.Id)
        {
            return BadRequest();
        }

        bool isUpdated = await _serviceManager.Actors.UpdateActorAsync(
            id,
            actorUpdateDto,
            cancellationToken
        );

        if (!isUpdated)
        {
            return NotFound();
        }

        return NoContent();
    }
}
