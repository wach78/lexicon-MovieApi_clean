using System;
using System.Collections.Generic;
using System.Text;
using Movie.Core.Entities;
using Movie.Core.Pagination;
using Movie.Core.Parameters;

namespace Movie.Core.DomainContracts;

public interface IActorRepository
{
    Task<PagedResult<Actor>> GetAllAsync(PaginationParameters paginationParameters, CancellationToken cancellationToken = default);
    Task<Actor?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Actor actor);
    void Update(Actor actor);
    void Remove(Actor actor);
}

