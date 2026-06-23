using Microsoft.AspNetCore.Mvc;
using Movie.Core.DTOs.Actor;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;

namespace MovieApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ActorsController : ControllerBase
{
    private readonly IActorService _actorService;

    public ActorsController(IActorService actorService)
    {
        ArgumentNullException.ThrowIfNull(actorService);

        _actorService = actorService;
    }

    // POST /api/movies/{movieId}/actors/{actorId}
    [HttpPost("/api/movies/{movieId:guid}/actors/{actorId:guid}")]
    public async Task<IActionResult> AddActorToMovie(
        [FromRoute] Guid movieId,
        [FromRoute] Guid actorId,
        CancellationToken cancellationToken = default)
    {
        AddActorToMovieResult result =
            await _actorService.AddActorToMovieAsync(
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
    public async Task<ActionResult<IReadOnlyList<ActorDto>>> GetActors(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ActorDto> actors =
            await _actorService.GetActorsAsync(cancellationToken);

        return Ok(actors);
    }

    // GET /api/actors/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ActorDto>> GetActor(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        ActorDto? actor = await _actorService.GetActorByIdAsync(
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
        ActorDto actor = await _actorService.CreateActorAsync(
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

        bool isUpdated = await _actorService.UpdateActorAsync(
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
