using Movie.Core.DTOs.Actor;
using Movie.Core.Pagination;
using Movie.Core.Parameters;
using Movie.Service.Contracts.Results;

namespace Movie.Service.Contracts.Interfaces;

public interface IActorService
{
    Task<PagedResult<ActorDto>> GetActorsAsync(
      PaginationParameters paginationParameters,
      CancellationToken cancellationToken = default
  );

    Task<ActorDto?> GetActorByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<ActorDto> CreateActorAsync(
        ActorCreateDto actorCreateDto,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateActorAsync(
        Guid id,
        ActorUpdateDto actorUpdateDto,
        CancellationToken cancellationToken = default
    );

    Task<AddActorToMovieResult> AddActorToMovieAsync(
        Guid movieId,
        Guid actorId,
        CancellationToken cancellationToken = default
    );
}
