using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Core.Models.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;

namespace Movie.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ActorsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public ActorsController(IServiceManager serviceManager)
    {
        ArgumentNullException.ThrowIfNull(serviceManager);

        _serviceManager = serviceManager;
    }

    // POST /api/movies/{movieId}/actors/{actorId}
    [HttpPost("/api/movies/{movieId:guid}/actors/{actorId:guid}")]
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

    // GET /api/actors
    [HttpGet]
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

    // GET /api/actors/{id}
    [HttpGet("{id:guid}")]
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

    // POST /api/actors
    [HttpPost]
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

    // PUT /api/actors/{id}
    [HttpPut("{id:guid}")]
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
