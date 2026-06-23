using Movie.Core.DomainContracts;
using Movie.Core.DTOs.Actor;
using Movie.Core.Entities;
using Movie.Service.Contracts.Interfaces;
using Movie.Service.Contracts.Results;
using MovieEntity = Movie.Core.Entities.Movie;

namespace Movie.Services;

public sealed class ActorService : IActorService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActorService(IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);

        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ActorDto>> GetActorsAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Actor> actors =
            await _unitOfWork.Actors.GetAllAsync(cancellationToken);

        return actors
            .Select(actor => new ActorDto
            {
                Id = actor.Id,
                Name = actor.Name,
                BirthYear = actor.BirthYear
            })
            .ToList();
    }

    public async Task<ActorDto?> GetActorByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        Actor? actor = await _unitOfWork.Actors.GetAsync(
            id,
            cancellationToken
        );

        if (actor is null)
        {
            return null;
        }

        return new ActorDto
        {
            Id = actor.Id,
            Name = actor.Name,
            BirthYear = actor.BirthYear
        };
    }

    public async Task<ActorDto> CreateActorAsync(
        ActorCreateDto actorCreateDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actorCreateDto);

        Actor actor = new(
            actorCreateDto.Name,
            actorCreateDto.BirthYear
        );

        _unitOfWork.Actors.Add(actor);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return new ActorDto
        {
            Id = actor.Id,
            Name = actor.Name,
            BirthYear = actor.BirthYear
        };
    }

    public async Task<bool> UpdateActorAsync(
        Guid id,
        ActorUpdateDto actorUpdateDto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(actorUpdateDto);

        Actor? actor = await _unitOfWork.Actors.GetAsync(
            id,
            cancellationToken
        );

        if (actor is null)
        {
            return false;
        }

        actor.Update(
            actorUpdateDto.Name,
            actorUpdateDto.BirthYear
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return true;
    }

    public async Task<AddActorToMovieResult> AddActorToMovieAsync(
        Guid movieId,
        Guid actorId,
        CancellationToken cancellationToken = default)
    {
        MovieEntity? movie = await _unitOfWork.Movies.GetWithActorsAsync(
            movieId,
            cancellationToken
        );

        if (movie is null)
        {
            return AddActorToMovieResult.MovieNotFound;
        }

        Actor? actor = await _unitOfWork.Actors.GetAsync(
            actorId,
            cancellationToken
        );

        if (actor is null)
        {
            return AddActorToMovieResult.ActorNotFound;
        }

        bool actorAlreadyAdded =
            movie.Actors.Any(existingActor => existingActor.Id == actorId);

        if (actorAlreadyAdded)
        {
            return AddActorToMovieResult.ActorAlreadyAdded;
        }

        movie.Actors.Add(actor);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return AddActorToMovieResult.Added;
    }
}
